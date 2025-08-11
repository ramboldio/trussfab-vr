using System.Collections.Generic;
using UnityEngine;

public class EdgeViewer : MonoBehaviour
{
    private View view;
    private List<GameObject> edges = new List<GameObject>();
    private int currentEdgeIndex = 0;

    void Awake()
    {
        view = GameObject.Find("Dino").GetComponent<View>();
        view.edgesDrawn.AddListener(loadEdges);
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Debug.Log("A button was just pressed on the right controller!");
            ShowNextEdge();
        }
    }

    private void loadEdges()
    {
        Debug.Log("edges count" + view.edges.Count);
        foreach (var e in view.edges)
        {
            Debug.Log("active edge" + e.edge.activeInHierarchy);

            edges.Add(e.edge);
        }

    }

    public void ShowNextEdge()
    {
        Debug.Log("edge number" + currentEdgeIndex + "edges count" + edges.Count);
        if (currentEdgeIndex < edges.Count)
        {
            edges[currentEdgeIndex].GetComponent<MeshRenderer>().enabled = true;
            Debug.Log("create edge number " + currentEdgeIndex + edges[currentEdgeIndex].GetComponent<MeshRenderer>().enabled);
            currentEdgeIndex++;
        }
        else
        {
            Debug.Log("✅ All edges are already visible.");
        }
    }

}
