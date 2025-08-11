using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform head;
    public Vector3 offset = new Vector3(0, 0, 1.5f); // 1.5 meters in front

    void Update()
    {
        if (head == null) return;
        transform.position = head.position + head.forward * offset.z;
        transform.rotation = Quaternion.LookRotation(transform.position - head.position);
    }
}
