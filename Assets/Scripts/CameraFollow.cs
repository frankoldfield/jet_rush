using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 lateralOffset = new Vector3(0, 5, -20);
    public Vector3 verticalOffset = new Vector3(0, 10, 0);

    void LateUpdate()
    {
        transform.position = target.position + lateralOffset;
        transform.LookAt(target);
        transform.position = transform.position + verticalOffset;
    }
}