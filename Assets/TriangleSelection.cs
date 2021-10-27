using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class TriangleSelection : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    [System.Serializable]
    public class AddGeometryEvent : UnityEvent<int[]> {}
    public static AddGeometryEvent addGeometryEvent = new AddGeometryEvent();

	private int[] linkedVertexIDs = null;
    private Plane plane = new Plane();

    private MeshCollider meshCollider = null;

    void Awake()
    {
        GetComponent<Renderer>().enabled = false;
    }

    public void OnPointerClick(PointerEventData data) {
        Vector3 position = data.pressEventCamera.transform.position;
        if (plane.GetSide(position)) {
            // click comes from the positive side of the plane
            addGeometryEvent.Invoke(this.linkedVertexIDs);
        } else {
            // click comes from below the plane -> revert the order to make sure that new geometry is added on the right side
            addGeometryEvent.Invoke(this.linkedVertexIDs.Reverse().ToArray());
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        GetComponent<Renderer>().enabled = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        GetComponent<Renderer>().enabled = false;
    }

    public void setTriangleCoordinates(int[] vertexIDs, Vector3[] triangleCorners) {
        // TODO ensure that this function was run before initializing the object
        Assert.AreEqual(triangleCorners.Length, 3);
        Assert.AreEqual(vertexIDs.Length, 3);

        plane.Set3Points(triangleCorners[0], triangleCorners[1], triangleCorners[2]);

        // TODO: check whether copying makes more sense here
		linkedVertexIDs = vertexIDs;

        Mesh mesh = new Mesh();
        mesh.vertices = triangleCorners.Concat(triangleCorners.Reverse()).ToArray();
        // two triangles with oposing normals
        mesh.triangles = new int[] { 0,1,2,3,4,5 };
        (gameObject.AddComponent<MeshFilter>() as MeshFilter).mesh = mesh;
        this.meshCollider = gameObject.AddComponent<MeshCollider>() as MeshCollider;
        meshCollider.sharedMesh = mesh;
    }
}