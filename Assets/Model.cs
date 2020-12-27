using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public struct Edge {
	public readonly int id;
	public readonly int v_src;
	public readonly int v_dest;

	public Edge(int id, int v_src, int v_dest) {
		this.id = id;
		this.v_src = v_src;
		this.v_dest = v_dest;
	}
}

public struct Vertex {
	public readonly int id;
	public readonly Vector3 pos;

	public Vertex(int id, Vector3 pos) {
		this.id = id;
		this.pos = pos;
	}
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
    			new Vertex(0, new Vector3(0,0,0)),
    			new Vertex(1, new Vector3(0,1,0)),
    			new Vertex(2, new Vector3(1,0,0)),
    			new Vertex(3, new Vector3(0.5f,0.5f,0.5f)),
    		},
    		new Edge[] {
    			//    id v_src v_dest
    			new Edge(0, 0, 1),
    			new Edge(1, 0, 2),
    			new Edge(2, 0, 3),
    			new Edge(3, 1, 2),
    			new Edge(4, 1, 3),
    			new Edge(5, 2, 3)
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

	public Vector3 getPosFromVertexID(int v_id) {
		return this.vertices[v_id].pos;
	}

	public static Graph addGeometry(Graph g, int[] triangle) {
		// TODO ensure that triangle has 3 entries

		// adds a new connected vertex to the structure that is placed outside the triangle by the specified distance 
		// direction in determined by the order of the vertecies in the triangle array (geometry is added on the clockwise side)
		float distanceFromTriangle = 0.5f;

		Plane buildingPlane = new Plane();
		buildingPlane.Set3Points(
			g.getPosFromVertexID(triangle[0]),
			g.getPosFromVertexID(triangle[1]),
			g.getPosFromVertexID(triangle[2])
		);
		Vector3 normal = buildingPlane.normal;

		// average over the corners
		Vector3 centroid = 
			triangle.Select(id => g.getPosFromVertexID(id)).Aggregate(new Vector3(), (acc, x) => acc + x) / triangle.Length;

		int newVertexId = g.vertices.Length;
		Vertex newVertex = new Vertex(newVertexId, normal * distanceFromTriangle + centroid);

		int newEdgeIdStart = g.edges.Length;
		List<Edge> newEdges = Enumerable.Range(0,2).Select(i => new Edge(newEdgeIdStart + i, newVertexId, triangle[i])).ToList();

		return new Graph(
			g.vertices.Append(newVertex).ToArray(),
			g.edges.Concat(newEdges).ToArray()
		);
	}
}

public class Model : MonoBehaviour
{
	public UnityEvent modelUpdate = new UnityEvent();

	// TODO: call this behavior 'controller'. The pure model code is above	
	private Graph graph;
    // Start is called before the first frame update
    void Start()
    {
    	// TriangleSelection.addGeometryEvent.AddEventListener(addGeometry);
    	// initialize Model with some dummy data (tetrahedron)
    	this.graph = Graph.getTetrahedron();
    	modelUpdate.Invoke();
    }

    public Graph getGraph() {
    	return graph;
    }

    public void addGeometry(int[] triangle) {
    	this.graph = Graph.addGeometry(graph, triangle);
    	modelUpdate.Invoke();
    }

}
