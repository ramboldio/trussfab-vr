using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.XR.CoreUtils;

public class CameraPositionDisplay : MonoBehaviour
{
    public Transform cameraTransform;     // XR Camera (CenterEyeAnchor)
    public Transform dinoTransform;       // Your Dino GameObject
    public View viewScript;               // Reference to View.cs (set in Inspector)

    public GameObject dino;
    private TextMeshPro textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        if (cameraTransform == null || textMesh == null) return;

        Vector3 camPos = cameraTransform.position;
        string displayText = $"📍 Camera Position:\nX: {camPos.x:F2} Y: {camPos.y:F2} Z: {camPos.z:F2}";

        // Dino position
        if (dinoTransform != null)
        {
            Vector3 dinoPos = dinoTransform.position;
            displayText += $"\n\n🦖 Dino Position:\nX: {dinoPos.x:F2} Y: {dinoPos.y:F2} Z: {dinoPos.z:F2}";
        }
        displayText += "\n" + dino.GetComponentsInChildren<Transform>().Length;

        // Graph vertices
        if (viewScript != null)
        {
            Dictionary<int, GameObject> verticesDict = viewScript.GetVertices();
            if (verticesDict != null && verticesDict.Count > 0)
            {
                displayText += "\n\n📌 Graph Vertices:";
                foreach (var kvp in verticesDict)
                {
                    Vector3 vPos = kvp.Value.transform.position;
                    displayText += $"\nV{kvp.Key}: X:{vPos.x:F1} Y:{vPos.y:F1} Z:{vPos.z:F1}";
                }
            }
            else
            {
                displayText += "\n\n(Graph has no vertices)";
            }
        }

        textMesh.text = displayText;

        // Position and rotate text to always face the camera
        transform.position = cameraTransform.position + cameraTransform.forward * 15.0f + cameraTransform.up * 0.1f;
        transform.position = new Vector3(transform.position.x + 8f, transform.position.y, transform.position.z);
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}
