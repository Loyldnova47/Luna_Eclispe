using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    // Drag your Player object into this slot in the Unity Inspector
    public Transform player;

    // This will automatically store the exact height you set in the editor
    private float lockedHeight;

    void Start()
    {
        // Remember the starting height of this camera
        lockedHeight = transform.position.y;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 1. Pure Tracking: Copy Player X and Z, use the stored editor height for Y
        Vector3 newPosition = new Vector3(player.position.x, lockedHeight, player.position.z);
        transform.position = newPosition;

        // 2. Lock Rotation: Point straight down, match player horizontal turning
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
