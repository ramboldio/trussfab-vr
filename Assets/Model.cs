using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Edge {
	public int id;
	public int v_src;
	public int v_dest;
}

public struct Vertex {
	public int id;
	public Vector3 pos;
} 

public struct Graph {
	public readonly Vertex[] vertices;
	public readonly Edge[] edges;

	public Graph(Vertex[] vertices, Edge[] edges) {
		this.vertices = vertices;
		this.edges = edges;
	}

	public static Graph getTetrahedron() {
		return new Graph (
    		 new Vertex[] {
    			new Vertex { id = 0, pos = new Vector3(0,0,0) },
    			new Vertex { id = 1, pos = new Vector3(0,1,0) },
    			new Vertex { id = 2, pos = new Vector3(1,0,0) },
    			new Vertex { id = 3, pos = new Vector3(0.5f,0.5f,0.5f) },
    		},
    		new Edge[] {
    			new Edge { id = 0, v_src = 0, v_dest = 1 },
    			new Edge { id = 1, v_src = 0, v_dest = 2 },
    			new Edge { id = 2, v_src = 0, v_dest = 3 },
    			new Edge { id = 3, v_src = 1, v_dest = 2 },
    			new Edge { id = 4, v_src = 1, v_dest = 3 },
    			new Edge { id = 5, v_src = 2, v_dest = 3 }
    		}
    	);
	}

	public bool[,] getAdjecencyMatrix () {
		bool[,] result = new bool[this.vertices.Length, this.vertices.Length];
		foreach (Edge e in this.edges){
			result[e.v_dest, e.v_src] = true;
			result[e.v_src, e.v_dest] = true;
		}
		return result;
	}

	// TODO return List instead
	public List<int[]> getTriangles () {
		List<int[]> triangles = new List<int[]>();
		bool[,] adjMatrix = getAdjecencyMatrix();
		Debug.Log(this.edges.Length);
		for (int i = 0; i < this.edges.Length - 1; i++){
			for (int j = i + 1; j < this.edges.Length; j++){
				// if there are two edges that meet in one point, we are checkin whether the other ends on these edges are connected.
				// if that is the case, we found a triangle
				if (this.edges[i].v_src == this.edges[j].v_src && 
					adjMatrix[this.edges[i].v_dest, this.edges[j].v_dest]) {
					triangles.Add(new int[] {this.edges[i].v_src, this.edges[i].v_dest, this.edges[j].v_dest});
				} else if (this.edges[i].v_src == this.edges[j].v_dest &&
					 adjMatrix[this.edges[i].v_src, this.edges[j].v_dest]) {
					triangles.Add(new int[] {this.edges[i].v_src, this.edges[i].v_dest, this.edges[j].v_src});
				}
			}
		}
		return triangles;
	}
}

public class Model : MonoBehaviour
{
	private Graph graph;
    // Start is called before the first frame update
    void Start()
    {
    	// initialize Model with some dummy data (tetrahedron)
    	this.graph = Graph.getTetrahedron();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Graph getGraph() {
    	return graph;
    }
}
