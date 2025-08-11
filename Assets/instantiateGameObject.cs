using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instantiateGameObject : MonoBehaviour
{
    private Model model;

    public GameObject model_GO;
    // Start is called before the first frame update
    public GameObject prefab;
    public GameObject vertexPrefab;
    public Transform cameraPos;
    void Awake()
    {
        model = model_GO.GetComponent<Model>();
        model.modelUpdate.AddListener(updateTrussStructure);
    }
    void Start()
    {


    }

    void updateTrussStructure()
    {
        Graph graph = model.getGraph();

        // drawVertex(new Vector3(0, 2, 2));
        for (int i = 0; i < graph.vertices.Length; i++)
        {
            
        drawVertex(graph.vertices[i].pos);
        }
        
        
        Physics.SyncTransforms();

    }


    // Update is called once per frame
    // void Update()
    // {
    //     Debug.Log(transform.localPosition);

    // }
    public void drawVertex(Vector3 pos)
    {
        GameObject obj = (GameObject)Instantiate(vertexPrefab, pos, Quaternion.identity);
        obj.transform.SetParent(this.transform);
        Debug.Log("Camera world Pos:" + cameraPos.position);
        Debug.Log("Camera world Pos:" + cameraPos.localPosition);
        Debug.Log("Vertex world pos:" + obj.transform.position);
        Debug.Log("Vertex local pos:" + obj.transform.localPosition);

    }
}
