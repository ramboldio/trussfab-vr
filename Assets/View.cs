using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{

    private Model model;
    private Graph currentlyActiveGraph;

    public GameObject vertexPrefab;
    public GameObject edgePrefab;
    public GameObject buildingColliderPrefab;

    // Start is called before the first frame update
    void Start()
    {
        model = GameObject.Find("Model").GetComponent<Model>(); 
        updateTrussStructure();
    }

    // Update is called once per frame
    void Update()
    {
        // updateTrussStructure();
    }


    void updateTrussStructure () {
        Graph graph = model.getGraph();

       // if (currentlyActiveGraph.Equals(graph)) {
       //  // already up-to-date
       //  return;
       // }
       
       clearAllChildren();

        for (int i = 0; i < graph.edges.Length; i++) {
            drawTruss(graph.vertices[graph.edges[i].v_src].pos, graph.vertices[graph.edges[i].v_dest].pos);
        }

        for (int i = 0; i < graph.vertices.Length; i++) {
            drawVertex(graph.vertices[i].pos);
        }

        this.drawCollisionBody();
        currentlyActiveGraph = graph;
    }

    void drawCollisionBody() {
        // find triangles
        Graph graph = model.getGraph();
        List<int []> triangles = graph.getTriangles();

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[graph.vertices.Length];
        for (int i = 0; i < graph.vertices.Length; i++) {
            vertices[i] = graph.vertices[i].pos;
        }
        mesh.vertices = vertices;

        int[] tris = new int[3*triangles.Count];
        for (int i = 0; i < triangles.Count; i++){
            tris[i*3 + 0] = triangles[i][0];
            tris[i*3 + 1] = triangles[i][1];
            tris[i*3 + 2] = triangles[i][2];
        }
        mesh.triangles = tris;

        meshFilter.mesh = mesh;
    }

    // void drawSelectionTriangles(Vector3[] triangleCorners) {
    //     gameObject = new GameObject();
    //     Instantiate(gameObject, transform.position, Quaternion.identity);
    // }
 
    void drawVertex(Vector3 pos) {
        GameObject obj = (GameObject) Instantiate(vertexPrefab, pos, Quaternion.identity);
        obj.transform.SetParent(this.transform);
    }

    void drawTruss(Vector3 p1, Vector3 p2) {
        Vector3 pos = Vector3.Lerp(p1,p2,(float)0.5);
        GameObject obj = (GameObject) Instantiate(edgePrefab, pos, Quaternion.identity);

        obj.transform.SetParent(this.transform);
        Vector3 newScale = obj.transform.localScale;
        newScale.y = Vector3.Distance(p1,p2)/2;
        obj.transform.localScale = newScale;
        obj.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p1 - p2);
    }

    void clearAllChildren() {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
         }
    }
}
