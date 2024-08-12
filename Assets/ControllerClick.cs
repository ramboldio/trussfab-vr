using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.Attributes;
using UnityEngine;

public class ControllerClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool lastTrigger = false;

    public Material actuatedMat;

    // Update is called once per frame
    void Update()
    {
       /* var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

        bool triggerValue;
        if (rightHandDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue))
        {
            if (triggerValue != lastTrigger) {
                lastTrigger = triggerValue;

                if (triggerValue) {
                    Debug.Log("TRIGGER");

                    if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 10)) {
                        if (hitInfo.collider.gameObject.tag == "HalfEdge") {
                            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().material = actuatedMat;
                        }
                    }
                }
            }
        }*/
    }
}
