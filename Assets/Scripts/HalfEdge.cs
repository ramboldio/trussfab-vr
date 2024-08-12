using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfEdge : MonoBehaviour
{
    public HalfEdge other;

    public bool isActuator;

    private Material defaultMaterial;
    public Material actuatedMaterial;

    public ConfigurableJoint joint = null;

    public Rigidbody endNode;

    float forceTimer = 0;

    void Start() {
        defaultMaterial = GetComponent<MeshRenderer>().material;
    }

    void Update() {
        if (!isActuator) {
            if (forceTimer > 0) {
                Vector3 forceDir = (endNode.position - other.endNode.position).normalized;
                endNode.AddForce(forceDir * 400);
                forceTimer -= Time.deltaTime;
            } else {
                if (joint) {
                    joint.angularXMotion = ConfigurableJointMotion.Locked;
                    joint.angularYMotion = ConfigurableJointMotion.Locked;
                    joint.angularZMotion = ConfigurableJointMotion.Locked;
                }

                forceTimer = 0;
            }
        }
    }


    [ContextMenu("ToggleActuation")]
    public void ToggleActuated() {
        if (isActuator) DisableActuation();
        else EnableActuation();
    }

    public void DisableActuation()
    {
        GetComponent<MeshRenderer>().material = defaultMaterial;
        
        forceTimer = 1;
        isActuator = false;
        if (other.isActuator) other.DisableActuation();
    }

    public void EnableActuation()
    {
        GetComponent<MeshRenderer>().material = actuatedMaterial;

        if (joint) {
            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Free;
        }

        isActuator = true;
        if (!other.isActuator) other.EnableActuation();
    }
}
