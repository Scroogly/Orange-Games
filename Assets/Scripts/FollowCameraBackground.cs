using UnityEngine;

public class FollowCameraBackground : MonoBehaviour
{
    public Transform cam;
    private Vector3 offset;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        // Keep the initial offset so we don’t change your Z value
        offset = transform.position - cam.position;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        transform.position = cam.position + offset;
    }
}
