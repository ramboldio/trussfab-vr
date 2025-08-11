using UnityEngine;
using System.Collections;
using Meta.XR.MRUtilityKit;

public class MRUKPhysicsSetup : MonoBehaviour
{
    [System.Obsolete]
    void Start()
    {
        StartCoroutine(WaitForRoomAndAddColliders());
    }

    [System.Obsolete]
    IEnumerator WaitForRoomAndAddColliders()
    {
        while (MRUK.Instance == null || MRUK.Instance.GetCurrentRoom() == null)
            yield return null;

        var room = MRUK.Instance.GetCurrentRoom();

        foreach (var anchor in room.Anchors)
        {
            if (IsFloor(anchor))
            {
                var mf = anchor.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    var mc = anchor.GetComponent<MeshCollider>();
                    if (mc == null) mc = anchor.gameObject.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                    mc.convex = false;
                }
            }
        }
    }

    [System.Obsolete]
    static bool IsFloor(MRUKAnchor anchor)
    {
        return anchor.HasLabel(MRUKAnchor.SceneLabels.FLOOR+"");
    }
}
