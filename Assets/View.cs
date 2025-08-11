using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class View : MonoBehaviour
{
    private Model model;

    public GameObject model_GO;
    public GameObject vertexPrefab;
    public GameObject edgePrefab;
    private Dictionary<int, GameObject> vertecies = new Dictionary<int, GameObject>();
    public List<edgeStartEnd> edges = new List<edgeStartEnd>();

    public UnityEvent edgesDrawn = new UnityEvent();


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
    void Awake()
    {
        model = model_GO.GetComponent<Model>();
        model.modelUpdate.AddListener(updateTrussStructure);
    }
    void Update()
    {
        Debug.Log("Dino location: " + transform.localPosition);
    }
    public Dictionary<int, GameObject> GetVertices()
    {
        return vertecies;
    }


    void updateTrussStructure()
    {
        Graph graph = model.getGraph();
        clearAllChildren();
        for (int i = 0; i < graph.edges.Length; i++)
        {
            var edge = drawTruss(graph.vertices[graph.edges[i].v_src].pos, graph.vertices[graph.edges[i].v_dest].pos);
            edges.Add(new edgeStartEnd(graph.vertices[graph.edges[i].v_src].id, graph.vertices[graph.edges[i].v_dest].id, edge));
        }

        // for (int i = 0; i < graph.vertices.Length; i++)
        // {

        //     var vertex = drawVertex(graph.vertices[i].pos);
        //     vertecies.Add(graph.vertices[i].id, vertex);
        // }
        edgesDrawn.Invoke();
        // transform.position = cameraTransform.position + cameraTransform.forward * 2;
        
    }

    // public GameObject drawVertex(Vector3 pos)
    // {
    //     GameObject obj = (GameObject)Instantiate(vertexPrefab, pos, Quaternion.identity);
    //     obj.transform.SetParent(this.transform);

    //     // obj.SetActive(false);
    //     Debug.Log("Vertex world pos:" + obj.transform.position);
    //     Debug.Log("Vertex local pos:" + obj.transform.localPosition);
    //     return obj;
    // }

    public GameObject drawTruss(Vector3 p1, Vector3 p2)
    {
        Vector3 pos = Vector3.Lerp(p1, p2, (float)0.5);
        GameObject obj = (GameObject)Instantiate(edgePrefab, pos, Quaternion.identity);

        obj.transform.SetParent(this.transform);
        Vector3 newScale = obj.transform.localScale;
        newScale.y = Vector3.Distance(p1, p2) / 2;
        obj.transform.localScale = newScale;
        obj.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p1 - p2);
        // obj.SetActive(false);
        obj.GetComponent<MeshRenderer>().enabled = false;

        return obj;
    }

    void clearAllChildren()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    // void addJoints()
    // {
    //     for (int i = 0; i < edges.Count; i++)
    //     {

    //         var edgeVertex1 = edges[i].edge.AddComponent<ConfigurableJoint>();
    //         var edgeVertex2 = edges[i].edge.AddComponent<ConfigurableJoint>();
    //         edgeVertex1.connectedBody = vertecies[edges[i].startId].GetComponent<Rigidbody>();
    //         edgeVertex1.anchor = new Vector3(0, 1, 0);
    //         edgeVertex1.xMotion = ConfigurableJointMotion.Locked;
    //         edgeVertex1.yMotion = ConfigurableJointMotion.Locked;
    //         edgeVertex1.zMotion = ConfigurableJointMotion.Locked;
    //         edgeVertex2.connectedBody = vertecies[edges[i].endId].GetComponent<Rigidbody>();
    //         edgeVertex2.anchor = new Vector3(0, -1, 0);
    //         edgeVertex2.xMotion = ConfigurableJointMotion.Locked;
    //         edgeVertex2.yMotion = ConfigurableJointMotion.Locked;
    //         edgeVertex2.zMotion = ConfigurableJointMotion.Locked;

    //     }
    // }
}