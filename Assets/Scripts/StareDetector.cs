using UnityEngine;

public class StareDetector : MonoBehaviour
{
    private PaintingStareEffect lastStaredPainting;
    private float teleportTimer = 0f;
    private const float TELEPORT_THRESHOLD = 7.0f;   

    void Update()
    {
        HandleStareDetection();
    }

    private void HandleStareDetection()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 50f))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);

            if (hit.collider.TryGetComponent<PaintingStareEffect>(out var painting))
            {
                painting.OnStare(Time.deltaTime);
                lastStaredPainting = painting;

                teleportTimer += Time.deltaTime;

                if (teleportTimer >= TELEPORT_THRESHOLD)
                {
                    TriggerSuddenTeleport();
                }
                return;
            }

        }

        // Looked away
        if (lastStaredPainting != null)
        {
            lastStaredPainting.OnLookAway();
            lastStaredPainting = null;
        }

        teleportTimer = 0f;
    }

    private void TriggerSuddenTeleport()
    {
        teleportTimer = 0f;

        if (lastStaredPainting != null)
        {
            lastStaredPainting.OnLookAway();
            lastStaredPainting = null;
        }

        // Find the XR Rig (Player)
        Transform xrRig = transform.root;

        Vector3 currentPos = xrRig.position;
        Vector3 randomOffset = new Vector3(
              Random.Range(-5f, 5f), 
              0f, 
              Random.Range(-5f, 5f)
   );
        Vector3 newPos = currentPos + randomOffset;
        newPos.y = currentPos.y;

        StartCoroutine(DoQuickTeleport(xrRig, newPos));
    }

    private System.Collections.IEnumerator DoQuickTeleport(Transform rig, Vector3 targetPos)
    {
        Vector3 startPos = rig.position;
        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rig.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }

        rig.position = targetPos;
    }
}
