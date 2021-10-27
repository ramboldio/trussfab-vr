using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // void Update() {
    //     // TODO add highlighting
    //     if (Input.GetMouseButtonDown(0)) {
    //         RaycastHit hitInfo = new RaycastHit();
    //         int layerMask = 1 << 8;
    //         bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, layerMask);
    //         if (hit) {
    //             try {
    //                 hitInfo.transform.gameObject.GetComponent<IPointerClickHandler>().OnClick();
    //             }
    //             catch { Debug.Log ("It's not working!"); };
    //         }
    //      } 

    //       if (OVRInput.GetDown(OVRInput.Button.One)) {
    //         RaycastHit hitInfo = new RaycastHit();
    //         int layerMask = 1 << 8;
    //         bool hit = Physics.Raycast(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand), Vector3.forward, out hitInfo, Mathf.Infinity, layerMask);
    //         if (hit) {
    //             try {
    //                 hitInfo.transform.gameObject.GetComponent<IPointerClickHandler>().OnClick();
    //             }
    //             catch { Debug.Log ("It's not working!"); };
    //         }
    //      } 
     // }
}
