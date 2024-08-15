using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public class HalfEdge : MonoBehaviour
{
    public HalfEdge other;

    public Actuator actuator;

    public bool isActuator;

    private Material defaultMaterial;
    public Material actuatedMaterial;

    public ConfigurableJoint joint = null;

    public Rigidbody endNode;

    float forceTimer = 0;

    public GameObject actuatorPrefab;



    void Start() {
        defaultMaterial = GetComponent<MeshRenderer>().material;

        if (!other.actuator) {
            GameObject go = Instantiate(actuatorPrefab, gameObject.transform);
            actuator = go.GetComponent<Actuator>();
            actuator.a = this;
        } else {
            actuator = other.actuator;
            actuator.b = this;
        }

        actuator.gameObject.SetActive(isActuator);
    }

    void Update() {
        if (!isActuator) {
            if (forceTimer > 0) {
                AddPressureForce(1);
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

    public void AddPressureForce(float factor) {
        Vector3 forceDir = (endNode.position - other.endNode.position).normalized;
        endNode.AddForce(factor * forceDir * 400);
        forceTimer -= Time.deltaTime;
    }


    [ContextMenu("ToggleActuation")]
    public void ToggleActuated() {
        if (isActuator) DisableActuation();
        else EnableActuation();
    }

    public void DisableActuation()
    {
        GetComponent<MeshRenderer>().material = defaultMaterial;
        
        forceTimer = 2;
        isActuator = false;
        actuator.gameObject.SetActive(false);
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
        actuator.gameObject.SetActive(true);
        if (!other.isActuator) other.EnableActuation();
    }

    public void EnableInput() {
        RayInteractor interactor = GetComponent<RayInteractable>().Interactors.First();

        Controller c = interactor.GetComponentInParent<Controller>();
        if (c) {
            actuator.EnableInput(c);
        }

    }

    public void DisableInput() {
        actuator.DisableInput();
    }
}
