using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPointer : OVRCursor
{
	public float defaultLength = 3.0f;

	private LineRenderer lineRenderer = null;

	void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	void Update()
	{
		// lineRenderer.SetPosition(0, transform.position);
		// lineRenderer.SetPosition(1, CalculateEnd());
	}

	Vector3 CalculateEnd()
	{
		RaycastHit hit = GetRayCast();
		Vector3 endPos = transform.position + transform.forward * defaultLength;

		if (hit.collider)
		{
			endPos = hit.point;
		}
		// Debug.log(endPos);
		return endPos;
	}

	RaycastHit GetRayCast()
	{
		RaycastHit hit;

		Ray ray = new Ray(transform.position, transform.forward);

		Physics.Raycast(ray, out hit);
		return hit;
	}

    public override void SetCursorRay(Transform ray)
    {

		lineRenderer.SetPosition(0, ray.position);
		lineRenderer.SetPosition(1, ray.position + ray.forward * defaultLength);
    }

    public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
    {
    	// TODO add normal for lineRenderer
		lineRenderer.SetPosition(0, start);
		lineRenderer.SetPosition(1, dest);
    }
}
