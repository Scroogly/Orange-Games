/*

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Drag the player here in the Inspector
    public float smoothSpeed = 0.125f;
    public Vector3 offset;    // Set this to adjust how far the camera sits behind the player

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
*/