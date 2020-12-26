using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class View : MonoBehaviour
{

    private Model model;
    private Graph currentlyActiveGraph;

    public GameObject vertexPrefab;
    public GameObject edgePrefab;
    public GameObject selectionSurfacePrefab;

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

        this.drawSelectionTriangles();
        currentlyActiveGraph = graph;
    }

    void drawSelectionTriangles() {
        Graph graph = model.getGraph();
        List<int []> triangles = graph.getTriangles();

        foreach (int[] triangle in triangles) {
            GameObject obj = (GameObject) Instantiate(selectionSurfacePrefab, new Vector3() , Quaternion.identity);
            TriangleSelection selectionSurface = obj.GetComponent<TriangleSelection>();
            selectionSurface.setTriangleCoordinates(
                triangle,
                triangle.Select((triangleVertexID) => graph.vertices[triangleVertexID].pos).ToArray()
            );

            // GameObject newTriangleColliderGo = makeSelectionTriangles(
            //     triangle.Select((triangleVertexID) => graph.vertices[triangleVertexID].pos).ToArray()
            // );
            obj.transform.SetParent(this.transform);
        }
    }

    static GameObject makeSelectionTriangles(Vector3[] triangleCorners) {
        Assert.AreEqual(triangleCorners.Length, 3);
        GameObject newGameObject = new GameObject();

        MeshRenderer meshRenderer = newGameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Selectii"));
        MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = triangleCorners;
        // two triangles with oposing normals
        mesh.triangles = new int[] { 0,1,2,2,1,0 };
        meshFilter.mesh = mesh;
        
        // add interaction behavior (highlighting on hover etc)
        newGameObject.AddComponent<TriangleSelection>();
        return newGameObject;
    }
 
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
