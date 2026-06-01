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
                return;
            }
        }

        if (lastStaredPainting != null)
        {
            lastStaredPainting.OnLookAway();
            lastStaredPainting = null;
        }
    }
}
