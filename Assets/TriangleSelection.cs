using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TriangleSelection : MonoBehaviour
{
	public int[] linkedVertexIDs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setTriangleCoordinates(int[] vertexIDs, Vector3[] triangleCorners) {
        Assert.AreEqual(triangleCorners.Length, 3);
        Assert.AreEqual(vertexIDs.Length, 3);

        // TODO: check whether copying makes more sense here
		linkedVertexIDs = vertexIDs;

        MeshFilter meshFilter = this.GetComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh.vertices = triangleCorners;
        // two triangles with oposing normals
        mesh.triangles = new int[] { 0,1,2,2,1,0 };
        meshFilter.mesh = mesh;
    }
}