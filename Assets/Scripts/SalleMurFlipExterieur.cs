using UnityEngine;

[DisallowMultipleComponent]
public class SalleMurFlipExterieur : MonoBehaviour
{
    public bool flipOnAwake = true;
    public bool recalcNormalsTangents = false;

    [Tooltip("Rend les meshes DOUBLE-FACE (visibles des 2 côtés) au lieu de simplement retourner les faces. " +
             "Corrige les murs qui n'apparaissent pas quelle que soit l'orientation des normales.")]
    public bool doubleSided = false;

    void Awake()
    {
        if (doubleSided) MakeDoubleSided();
        else if (flipOnAwake) Flip();
    }

    [ContextMenu("Make double-sided now")]
    public void MakeDoubleSided()
    {
        var filters = GetComponentsInChildren<MeshFilter>(true);
        int n = 0;
        foreach (var f in filters)
        {
            var src = f.sharedMesh;
            if (src == null) continue;
            var copy = Instantiate(src);
            copy.name = src.name + "_doubleSided";
            // Pour chaque sous-mesh : triangles d'origine + triangles inversés (même matériau).
            for (int sm = 0; sm < copy.subMeshCount; sm++)
            {
                var tris = copy.GetTriangles(sm);
                int len = tris.Length;
                var both = new System.Collections.Generic.List<int>(len * 2);
                both.AddRange(tris);
                for (int i = 0; i < len; i += 3)
                {
                    both.Add(tris[i]);
                    both.Add(tris[i + 2]);
                    both.Add(tris[i + 1]);
                }
                copy.SetTriangles(both, sm);
            }
            f.sharedMesh = copy;
            n++;
        }
        Debug.Log($"SalleMurFlipExterieur: double-sided {n} mesh(es) on '{name}'.");
    }

    [ContextMenu("Flip faces now")]
    public void Flip()
    {
        var filters = GetComponentsInChildren<MeshFilter>(true);
        int n = 0;
        foreach (var f in filters)
        {
            var src = f.sharedMesh;
            if (src == null) continue;
            var copy = Instantiate(src);
            copy.name = src.name + "_flipped";
            for (int sm = 0; sm < copy.subMeshCount; sm++)
            {
                var tris = copy.GetTriangles(sm);
                for (int i = 0; i < tris.Length; i += 3)
                {
                    int tmp = tris[i + 1];
                    tris[i + 1] = tris[i + 2];
                    tris[i + 2] = tmp;
                }
                copy.SetTriangles(tris, sm);
            }
            if (recalcNormalsTangents)
            {
                copy.RecalculateNormals();
                copy.RecalculateTangents();
            }
            else
            {
                var norms = copy.normals;
                if (norms != null && norms.Length > 0)
                {
                    for (int i = 0; i < norms.Length; i++) norms[i] = -norms[i];
                    copy.normals = norms;
                }
            }
            f.sharedMesh = copy;
            n++;
        }
        Debug.Log($"SalleMurFlipExterieur: flipped {n} mesh(es) on '{name}'.");
    }
}
