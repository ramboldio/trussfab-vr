using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPinch2Zoom : MonoBehaviour
{
	private bool userIsInteracting = false;
	private (Vector3, Vector3) startingHandPositions;
	public GameObject target = null;

    void Update()
    {	
    	if (userIsInteracting && BothButtonsArePressed()) {
    		SetTargetTransform(target.transform, startingHandPositions, GetCurrentHandPosTuple());
    	} else if (BothButtonsArePressed()) {
    		startingHandPositions = GetCurrentHandPosTuple();
    		userIsInteracting = true;
        } else {
			userIsInteracting = false;
        }
    }


    static void SetTargetTransform(Transform targetTransform, (Vector3, Vector3) start, (Vector3, Vector3) dest) {
    	targetTransform.localPosition = dest.Item1;
    	targetTransform.localRotation = Quaternion.FromToRotation(start.Item1 - start.Item2, dest.Item1 - dest.Item2);

    	float scalingDistance = Vector3.Distance(dest.Item1, dest.Item2) - Vector3.Distance(start.Item1, start.Item2);
    	targetTransform.localScale = new Vector3(scalingDistance, scalingDistance, scalingDistance);
    }

    (Vector3, Vector3) GetCurrentHandPosTuple() {
    	// TODO implement
    	return (
    		OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch),
    		OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch)
    	);
    }

    bool BothButtonsArePressed() {
    	return 
    		OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) == 1f && 
    		OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) == 1f;
    }
}
