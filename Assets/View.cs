using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.PlayerLoop;

public class View : MonoBehaviour
{
    private Model model;
    public GameObject plane;
    public GameObject model_GO;
    public GameObject vertexPrefab;
    // public GameObject edgePrefab;
    public GameObject selectionSurfacePrefab;
    public GameObject halfEdgePrefab;
    private Dictionary<int, GameObject> vertecies = new Dictionary<int, GameObject>();
    private List<edgeStartEnd> edges = new List<edgeStartEnd>();
    private List<GameObject> edgesPrefabs = new List<GameObject>();
    private GameObject inflationVertex;

    public double tubeWidth = 0.5;
    public class edgeStartEnd
    {
        public int startId;
        public int endId;
        public GameObject edge;
        public edgeStartEnd(int startId, int endId, GameObject edge)
        {
            this.startId = startId;
            this.endId = endId;
            this.edge = edge;
        }

    }
    void Start()
    {
        model = model_GO.GetComponent<Model>();
        updateTrussStructure();
        model.modelUpdate.AddListener(updateTrussStructure);
        // addJoints();

        foreach (var edge in edgesPrefabs)
        {
            edge.GetComponent<ConfigurableJoint>().autoConfigureConnectedAnchor = false;
        }
    }

    void Update()
    {
        //  print("inflation vertex pos:"+inflationVertex.transform.localPosition);
        if (Input.GetKey(KeyCode.Space))
        {
            // Apply a force to this Rigidbody in direction of this GameObjects up axis
            print("hi");
            inflationVertex.GetComponent<Rigidbody>().AddForce(Vector3.up * 100, ForceMode.Force);
        }
        // else{
        //     inflationVertex.GetComponent<Rigidbody>().
        // }
    }

    void updateTrussStructure()
    {
        Graph graph = model.getGraph();
        clearAllChildren();

        for (int i = 0; i < graph.vertices.Length; i++)
        {
            var vertex = drawVertex(graph.vertices[i].pos);
            if (graph.vertices[i].id == 3 || graph.vertices[i].id == 4 || graph.vertices[i].id == 5 ||graph.vertices[i].id == 1 || graph.vertices[i].id == 2 || graph.vertices[i].id == 6)
            {
                vertex.GetComponent<Rigidbody>().mass=90000;
            }
            vertecies.Add(graph.vertices[i].id, vertex);
        }
        for (int i = 0; i < graph.edges.Length; i++)
        {
            drawEdge(graph.vertices[graph.edges[i].v_src].pos, graph.vertices[graph.edges[i].v_dest].pos, graph.vertices[graph.edges[i].v_src].id, graph.vertices[graph.edges[i].v_dest].id);
            // edges.Add(new edgeStartEnd(graph.vertices[graph.edges[i].v_src].id, graph.vertices[graph.edges[i].v_dest].id, edge));
        }


        // this.drawSelectionTriangles(graph);
    }

    // void drawSelectionTriangles(Graph graph) {
    //     List<int []> triangles = graph.getTriangles();

    //     foreach (int[] triangle in triangles) {
    //         GameObject obj = (GameObject) Instantiate(selectionSurfacePrefab, new Vector3() , Quaternion.identity);
    //         TriangleSelection selectionSurface = obj.GetComponent<TriangleSelection>();
    //         selectionSurface.setTriangleCoordinates(
    //             triangle,
    //             triangle.Select((triangleVertexID) => graph.vertices[triangleVertexID].pos).ToArray()
    //         );

    //         obj.transform.SetParent(this.transform);

    //         // move obj to layer to be able to do a selective phyics raycast later e.g. in MouseSelector.cs
    //         obj.layer = 8;

    //     }
    // }

    public GameObject drawVertex(Vector3 pos)
    {
        GameObject obj = (GameObject)Instantiate(vertexPrefab, pos, Quaternion.identity);
        obj.transform.SetParent(this.transform);

        return obj;
    }

    public void drawEdge(Vector3 p1, Vector3 p2, int p1id, int p2id)
    {
        // Vector3 pos = Vector3.Lerp(p1, p2, (float)0.5);

        GameObject h1 = (GameObject)Instantiate(halfEdgePrefab, p1, Quaternion.identity);
        GameObject h2 = (GameObject)Instantiate(halfEdgePrefab, p2, Quaternion.identity);
        h1.GetComponent<ConfigurableJoint>().connectedBody = vertecies[p1id].GetComponent<Rigidbody>();
        h2.GetComponent<ConfigurableJoint>().connectedBody = vertecies[p2id].GetComponent<Rigidbody>();
        // h1.GetComponent<ConfigurableJoint>().autoConfigureConnectedAnchor=false;
        // h2.GetComponent<ConfigurableJoint>().autoConfigureConnectedAnchor=false;
        // h2.GetComponent<ConfigurableJoint>().anchor=new Vector3(0,1,0);
        h1.transform.SetParent(this.transform);
        h2.transform.SetParent(this.transform);
        var h1Joint = h1.AddComponent<ConfigurableJoint>();
        h1Joint.anchor = new Vector3(0, 0, 0);
        h1Joint.connectedBody = h2.GetComponent<Rigidbody>();
        h1Joint.xMotion = ConfigurableJointMotion.Locked;
        h1Joint.yMotion = ConfigurableJointMotion.Locked;
        h1Joint.zMotion = ConfigurableJointMotion.Locked;
        h1Joint.angularXMotion = ConfigurableJointMotion.Locked;
        h1Joint.angularYMotion = ConfigurableJointMotion.Locked;
        h1Joint.angularZMotion = ConfigurableJointMotion.Locked;
        h1Joint.autoConfigureConnectedAnchor = false;
        Vector3 newScale = new Vector3();
        print("tubeWidth" + tubeWidth);
        float diameter = (float)(tubeWidth * 2 / Math.PI);
        print("diameter" + diameter);
        newScale.y = Vector3.Distance(p1, p2) / 2;
        newScale.x = diameter;
        newScale.z = diameter;
        h1.transform.localScale = newScale;
        h2.transform.localScale = -newScale;

        h1.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p2 - p1);
        h2.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p2 - p1);
        edgesPrefabs.Add(h1);
        edgesPrefabs.Add(h2);

    }

    void clearAllChildren()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    void addJoints()
    {
        for (int i = 0; i < edges.Count; i++)
        {

            var left = edges[i].edge.transform.GetChild(0);
            var right = edges[i].edge.transform.GetChild(1);
            // var edgeVertex1 = edges[i].edge.AddComponent<ConfigurableJoint>();
            // var edgeVertex2 = edges[i].edge.AddComponent<ConfigurableJoint>();
            // var confArr=edges[i].edge.GetComponents<ConfigurableJoint>();
            var leftJoint = left.GetComponent<ConfigurableJoint>();
            leftJoint.connectedBody = vertecies[edges[i].startId].GetComponent<Rigidbody>();
            // leftJoint.anchor = new Vector3(0, 1, 0);
            // edgeVertex1.xMotion = ConfigurableJointMotion.Locked;
            // edgeVertex1.yMotion = ConfigurableJointMotion.Locked;
            // edgeVertex1.zMotion = ConfigurableJointMotion.Locked;
            var rightJoint = right.GetComponent<ConfigurableJoint>();
            rightJoint.connectedBody = vertecies[edges[i].endId].GetComponent<Rigidbody>();
            // rightJoint.anchor = new Vector3(0, -1, 0);
            // edgeVertex2.xMotion = ConfigurableJointMotion.Locked;
            // edgeVertex2.yMotion = ConfigurableJointMotion.Locked;
            // edgeVertex2.zMotion = ConfigurableJointMotion.Locked;

        }
    }
}