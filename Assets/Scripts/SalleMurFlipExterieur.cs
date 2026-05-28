using UnityEngine;

[DisallowMultipleComponent]
public class SalleMurFlipExterieur : MonoBehaviour
{
    public bool flipOnAwake = true;
    public bool recalcNormalsTangents = false;

    void Awake()
    {
        if (flipOnAwake) Flip();
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
