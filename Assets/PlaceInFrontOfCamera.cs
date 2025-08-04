
using UnityEngine;

public class PlaceInFrontOfCamera : MonoBehaviour
{

    void Start()
    {
        // transform.localPosition = new Vector3(-0.890424669f,3.2529161f,7.3038578f);
        // // transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
        Transform cameraTransform = GameObject.Find("CenterEyeAnchor").transform;
        float distanceInFront = 10f;
        Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * distanceInFront;
        transform.position = spawnPosition;
        transform.rotation = cameraTransform.rotation;
    }
    void Update()
    {
        Transform cameraTransform = GameObject.Find("CenterEyeAnchor").transform;
        Debug.Log("Dinocamera" + cameraTransform.position);
        Debug.Log("Dinodino"+transform.position);
    }
}
