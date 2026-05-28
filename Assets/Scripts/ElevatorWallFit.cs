using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ElevatorWallFit : MonoBehaviour
{
    public enum FitAxis { LocalForward, LocalBack, LocalRight, LocalLeft, WorldX, WorldNegX, WorldZ, WorldNegZ }

    [Tooltip("Direction to push the elevator until its renderer bounds collide with a wall.")]
    public FitAxis fitDirection = FitAxis.LocalBack;
    [Tooltip("Layer mask of walls (default: all layers).")]
    public LayerMask wallMask = ~0;
    [Tooltip("Max distance to search.")]
    public float maxDistance = 25f;
    [Tooltip("Small inset so elevator embeds into wall instead of leaving a gap.")]
    public float embedDistance = 0.05f;
    [Tooltip("Only consider colliders on these GameObjects (and their children). Leave empty for any.")]
    public Transform[] wallRoots;

    Vector3 GetDir()
    {
        switch (fitDirection)
        {
            case FitAxis.LocalForward: return transform.forward;
            case FitAxis.LocalBack:    return -transform.forward;
            case FitAxis.LocalRight:   return transform.right;
            case FitAxis.LocalLeft:    return -transform.right;
            case FitAxis.WorldX:       return Vector3.right;
            case FitAxis.WorldNegX:    return -Vector3.right;
            case FitAxis.WorldZ:       return Vector3.forward;
            default:                   return -Vector3.forward;
        }
    }

    [ContextMenu("Fit to wall now")]
    public void FitToWall()
    {
        var rends = GetComponentsInChildren<Renderer>();
        if (rends.Length == 0) { Debug.LogWarning("No renderers under " + name); return; }
        Bounds b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);

        Vector3 dir = GetDir().normalized;
        Vector3 outerFace = b.center + dir * (Vector3.Scale(b.extents, new Vector3(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z))).magnitude);
        Vector3 originPt = b.center;

        RaycastHit hit;
        bool hitFound = Physics.Raycast(outerFace - dir * 0.01f, dir, out hit, maxDistance, wallMask, QueryTriggerInteraction.Ignore);
        if (!hitFound)
        {
            Debug.LogWarning($"ElevatorWallFit on {name}: no wall hit in dir {dir}.");
            return;
        }
        if (wallRoots != null && wallRoots.Length > 0)
        {
            bool ok = false;
            foreach (var root in wallRoots)
            {
                if (root != null && hit.transform.IsChildOf(root)) { ok = true; break; }
            }
            if (!ok) { Debug.LogWarning($"Hit '{hit.transform.name}' not under wallRoots — skip."); return; }
        }

        float currentGap = hit.distance - 0.01f;
        Vector3 delta = dir * (currentGap + embedDistance);
        transform.position += delta;
        Debug.Log($"ElevatorWallFit on {name}: pushed by {delta.magnitude:F2}m into '{hit.transform.name}'.");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ElevatorWallFit))]
public class ElevatorWallFitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        if (GUILayout.Button("Fit to wall now"))
        {
            ((ElevatorWallFit)target).FitToWall();
            EditorUtility.SetDirty(((ElevatorWallFit)target).gameObject);
        }
    }
}
#endif
