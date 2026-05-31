using UnityEngine;
using System.Collections.Generic;

public class StareDetector : MonoBehaviour
{
    private PaintingStareEffect lastStaredPainting;
    private List<PaintingStareEffect> allPaintings = new List<PaintingStareEffect>();

    [Header("Peripheral Threshold")]
    [Tooltip("How directly you must look at a painting to count as a direct look (0.85 = close to center screen).")]
    public float directLookThreshold = 0.85f;

    private void Start()
    {
        // Find all active horror paintings in the room automatically at startup
        allPaintings.AddRange(FindObjectsByType<PaintingStareEffect>(FindObjectsSortMode.None));
    }

    private void Update()
    {
        HandlePeripheralAndDirectGaze();
    }

    private void HandlePeripheralAndDirectGaze()
    {
        PaintingStareEffect directlyLookedPainting = null;

        // 1. Fire the standard core Raycast to see if we are looking directly at a painting frame
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 50f))
        {
            if (hit.collider.TryGetComponent<PaintingStareEffect>(out var painting))
            {
                directlyLookedPainting = painting;
                
                // If it's a CreepyDistortion style, let it count its timers normally
                painting.OnStare(Time.deltaTime);

                if (painting != lastStaredPainting)
                {
                    lastStaredPainting = painting;
                }

                // Teleport after long direct stare
                if (Time.time % 6f < 0.1f) 
                {
                    TriggerSuddenTeleport();
                }
            }
        }

        // 2. Loop through all paintings in the scene to handle peripheral checks
        foreach (var painting in allPaintings)
        {
            if (painting == null) continue;

            // If we are already looking directly at it via Raycast, force it to stay normal
            if (painting == directlyLookedPainting)
            {
                painting.UpdatePeripheralGaze(transform, directLookThreshold, true);
            }
            else
            {
                // Otherwise, calculate its position relative to our turning angle
                painting.UpdatePeripheralGaze(transform, directLookThreshold, false);
            }
        }

        // Clean up direct gaze reference if we look into empty space
        if (directlyLookedPainting == null && lastStaredPainting != null)
        {
            lastStaredPainting.OnLookAway();
            lastStaredPainting = null;
        }
    }

    private void TriggerSuddenTeleport()
    {
        if (lastStaredPainting != null)
            lastStaredPainting.OnLookAway();

        Transform xrRig = transform.root;
        Vector3 currentPos = xrRig.position;

        Vector3 randomOffset = new Vector3(
            Random.Range(-8f, 8f),
            Random.Range(-0.3f, 0.3f),
            Random.Range(-9f, -2f)   
        );

        Vector3 newPos = currentPos + randomOffset;
        newPos.y = currentPos.y;

        StartCoroutine(DoQuickTeleport(xrRig, newPos));
    }

    private System.Collections.IEnumerator DoQuickTeleport(Transform rig, Vector3 targetPos)
    {
        Vector3 startPos = rig.position;
        float duration = 0.32f;
        float elapsed = 0f;

        Vector3 originalCamPos = Camera.main.transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float shake = (1f - t) * 0.15f;
            Camera.main.transform.localPosition = originalCamPos + new Vector3(
                Random.Range(-shake, shake),
                Random.Range(-shake, shake),
                0
            );

            rig.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rig.position = targetPos;
        Camera.main.transform.localPosition = originalCamPos;
    }
}
