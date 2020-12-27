using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class TriangleSelection : EventTrigger
{

    [System.Serializable]
    public class AddGeometryEvent : UnityEvent<int[]> {}
    public static AddGeometryEvent addGeometryEvent = new AddGeometryEvent();

	public int[] linkedVertexIDs = null;
    public MeshFilter meshFilter = null;
    public MeshCollider meshCollider = null;

    public override void OnPointerClick(PointerEventData data) {
        addGeometryEvent.Invoke(this.linkedVertexIDs);
    }


    public void setTriangleCoordinates(int[] vertexIDs, Vector3[] triangleCorners) {
        Assert.AreEqual(triangleCorners.Length, 3);
        Assert.AreEqual(vertexIDs.Length, 3);

        // TODO: check whether copying makes more sense here
		linkedVertexIDs = vertexIDs;

        Mesh mesh = new Mesh();
        mesh.vertices = triangleCorners;
        // two triangles with oposing normals
        mesh.triangles = new int[] { 0,1,2,2,1,0 };
        GetComponent<MeshFilter>().mesh = mesh;
        // meshCollider.mesh = mesh;
    }
}