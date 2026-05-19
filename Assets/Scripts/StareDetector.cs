using UnityEngine;

public class StareDetector : MonoBehaviour
{
    private PaintingStareEffect lastStaredPainting;

    private void Update()
    {
        HandleStareDetection();
    }

    private void HandleStareDetection()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 50f))
        {
            if (hit.collider.TryGetComponent<PaintingStareEffect>(out var painting))
            {
                painting.OnStare(Time.deltaTime);
                lastStaredPainting = painting;

                // Teleport after long stare (you can adjust number)
                if (Time.time % 6f < 0.1f) // rough every ~6 seconds of staring
                {
                    TriggerSuddenTeleport();
                }
                return;
            }
        }

        if (lastStaredPainting != null)
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

        // Big teleport, biased backwards/sideways
        Vector3 randomOffset = new Vector3(
            Random.Range(-8f, 8f),
            Random.Range(-0.3f, 0.3f),
            Random.Range(-9f, -2f)   // mostly backwards
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
