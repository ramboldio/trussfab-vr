using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.WitAi;
using UnityEngine;
using UnityEngine.Assertions;

public class configureJointModel : MonoBehaviour
{
    private Model model;

    public GameObject model_GO;
    public GameObject vertexPrefab;
    public GameObject edgePrefab;
    public GameObject selectionSurfacePrefab;
    private static List<GameObject> edges = new List<GameObject>();
    private static List<GameObject> vertecies = new List<GameObject>();
    private Rigidbody inflatedEdge, inflatingVertex1, inflatingVertex2;
    private static List<GameObject> relatedEdges = new List<GameObject>();
    private static List<String> relatedEdgesTopOrBottom = new List<string>();
    private static List<Vector3> fixedVerteciesrelatedEdges = new List<Vector3>();
    
    void Start()
    {
        model = model_GO.GetComponent<Model>();
        updateTrussStructure();
        model.modelUpdate.AddListener(updateTrussStructure);



    }

    // void addJoint()
    // {
    //     for (int i = 0; i < edges.Count; i++)
    //     {
    //         var confJoint=edges[i].AddComponent<ConfigurableJoint>();
    //         confJoint.xMotion=ConfigurableJointMotion.Locked;
    //         confJoint.yMotion=ConfigurableJointMotion.Locked;
    //         confJoint.zMotion=ConfigurableJointMotion.Locked;
            
    //     }

    // }

    void drawEdge(Vector3 p1, Vector3 p2)
    {
        Vector3 pos = Vector3.Lerp(p1, p2, (float)0.5);
        GameObject segObj = (GameObject)Instantiate(edgePrefab);
        segObj.transform.position = pos;
        segObj.transform.LookAt(p2);
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }
    void updateTrussStructure()
    {
        Graph graph = model.getGraph();
        clearAllChildren();

        for (int i = 0; i < graph.edges.Length; i++)
        {
            drawTruss(graph.vertices[graph.edges[i].v_src].pos, graph.vertices[graph.edges[i].v_dest].pos);
        }

        for (int i = 0; i < graph.vertices.Length; i++)
        {
            drawVertex(graph.vertices[i].pos);
        }

        this.drawSelectionTriangles(graph);
    }

    void drawSelectionTriangles(Graph graph)
    {
        List<int[]> triangles = graph.getTriangles();

        foreach (int[] triangle in triangles)
        {
            GameObject obj = (GameObject)Instantiate(selectionSurfacePrefab, new Vector3(), Quaternion.identity);
            TriangleSelection selectionSurface = obj.GetComponent<TriangleSelection>();
            selectionSurface.setTriangleCoordinates(
                triangle,
                triangle.Select((triangleVertexID) => graph.vertices[triangleVertexID].pos).ToArray()
            );

            obj.transform.SetParent(this.transform);

            // move obj to layer to be able to do a selective phyics raycast later e.g. in MouseSelector.cs
            obj.layer = 8;

        }
    }

    void drawVertex(Vector3 pos)
    {
        GameObject obj = (GameObject)Instantiate(vertexPrefab, pos, Quaternion.identity);
        vertecies.Add(obj);
        obj.transform.SetParent(this.transform);
    }

    void drawTruss(Vector3 p1, Vector3 p2)
    {
        Vector3 pos = Vector3.Lerp(p1, p2, (float)0.5);
        GameObject obj = (GameObject)Instantiate(edgePrefab, pos, Quaternion.identity);
        edges.Add(obj);
        obj.transform.SetParent(this.transform);
        Vector3 newScale = obj.transform.localScale;
        newScale.y = Vector3.Distance(p1, p2) / 2;
        obj.transform.localScale = newScale;
        obj.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p1 - p2);
    }

    void clearAllChildren()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
