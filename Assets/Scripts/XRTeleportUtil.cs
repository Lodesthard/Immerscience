using UnityEngine;
using Unity.XR.CoreUtils;

public static class XRTeleportUtil
{
    public static XROrigin FindRig(Transform any)
    {
        return any ? any.GetComponentInParent<XROrigin>() : null;
    }

    // Teleports the rig so the player's HEAD (not the rig pivot) lands on the
    // marker. Corrects the head's horizontal offset from the rig pivot —
    // skipping this drops the player off-mark when they've moved inside their
    // play space. The player's look direction is preserved (marker rotation is
    // NOT forced). flipYaw180 spins the player 180° about world up so they end
    // up facing the opposite way (face-to-face elevators).
    public static void TeleportToMarker(XROrigin rig, Transform marker, bool flipYaw180, bool keepHeight = false)
    {
        float startY = rig.transform.position.y;

        rig.transform.position = marker.position;

        if (flipYaw180)
            rig.transform.Rotate(0f, 180f, 0f, Space.World);

        Vector3 headOffset = rig.Camera.transform.position - rig.transform.position;
        headOffset.y = 0f;
        rig.transform.position -= headOffset;

        if (keepHeight)
        {
            Vector3 p = rig.transform.position;
            p.y = startY;
            rig.transform.position = p;
        }
    }
}
