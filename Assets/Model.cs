using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;

public struct Edge
{
	public readonly int id;
	public readonly int v_src;
	public readonly int v_dest;
	public bool actuator;

	public Edge(int id, int v_src, int v_dest,bool actuator)
	{
		this.id = id;
		this.v_src = v_src;
		this.v_dest = v_dest;
		this.actuator=actuator;
	}
}

public struct Vertex
{
	public readonly int id;
	public readonly Vector3 pos;
	public bool inflationVertex;
	public Vertex(int id, Vector3 pos,bool inflationVertex)
	{
		this.id = id;
		this.pos = pos;
		this.inflationVertex=inflationVertex;
	}
}

public struct Graph
{
	public readonly Vertex[] vertices;
	public readonly Edge[] edges;

	public Graph(Vertex[] vertices, Edge[] edges)
	{
		this.vertices = vertices;
		this.edges = edges;
	}

	public static Graph InitTetrahedron()
	{
		Vertex[] dinoVertecies = new Vertex[7];
		Edge[] dinoEdges = new Edge[15];
		//reading dino indecies
		const Int32 BufferSize = 128;
		using (var fileStream = File.OpenRead("Assets/camera.obj"))
		using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
		{
			String line;
			int vertexId = 0;
			int edgeId=0;
			while ((line = streamReader.ReadLine()) != null)
			{
				string[] lineArr = line.Split(" ");
				if (line.StartsWith("v"))
				{

					Vector3 vertexCoordinates = new Vector3(float.Parse(lineArr[1]), float.Parse(lineArr[2]), float.Parse(lineArr[3]));
					bool inflation=false;
					if(lineArr.Length>=5 && lineArr[4].Equals("inflation1")){
						inflation=true;
					}
					dinoVertecies[vertexId] = new Vertex(vertexId++, vertexCoordinates,inflation);

				}
				if (line.StartsWith("l"))
				{
					int vertex1 = Int32.Parse(lineArr[1]);
					int vertex2 = Int32.Parse(lineArr[2]);
					if(edgeId==7){
					dinoEdges[edgeId] = new Edge(edgeId++, vertex1-1, vertex2-1,true);
					}
					else{
						dinoEdges[edgeId] = new Edge(edgeId++, vertex1-1, vertex2-1,false);
					}
				}
			}
		}
		return new Graph(dinoVertecies,dinoEdges);
	}

	public bool[,] getAdjecencyMatrix()
	{
		bool[,] result = new bool[this.vertices.Length, this.vertices.Length];
		foreach (Edge e in this.edges)
		{
			result[e.v_dest, e.v_src] = true;
			result[e.v_src, e.v_dest] = true;
		}
		return result;
	}

	// TODO return List instead
	public List<int[]> getTriangles()
	{
		List<int[]> triangles = new List<int[]>();
		bool[,] adjMatrix = getAdjecencyMatrix();
		for (int i = 0; i < this.edges.Length - 1; i++)
		{
			for (int j = i + 1; j < this.edges.Length; j++)
			{
				// if there are two edges that meet in one point, we are checkin whether the other ends on these edges are connected.
				// if that is the case, we found a triangle
				if (this.edges[i].v_src == this.edges[j].v_src &&
					adjMatrix[this.edges[i].v_dest, this.edges[j].v_dest])
				{
					triangles.Add(new int[] { this.edges[i].v_src, this.edges[i].v_dest, this.edges[j].v_dest });
				}
				else if (this.edges[i].v_src == this.edges[j].v_dest &&
					 adjMatrix[this.edges[i].v_src, this.edges[j].v_dest])
				{
					triangles.Add(new int[] { this.edges[i].v_src, this.edges[i].v_dest, this.edges[j].v_src });
				}
			}
		}
		return triangles;
	}

	public Vector3 GetPosFromVertexID(int v_id)
	{
		return this.vertices[v_id].pos;
	}

	public static Graph MergeCloseNeighbours(Graph g)
	{
		// TODO implement
		// List<Vertex> vertecies = g.vertecies.Select(a => a).ToList<Vertex>();
		// List<Edge> vertecies = g.vertecies.Select(a => a).ToList<Edges>();
		return g;
	}


	public static Graph AddGeometry(Graph g, int[] triangle)
	{
		// TODO ensure that triangle has 3 entries

		// adds a new connected vertex to the structure that is placed outside the triangle by the specified distance 
		// direction in determined by the order of the vertecies in the triangle array (geometry is added on the clockwise side)
		float distanceFromTriangle = 0.5f;

		Plane buildingPlane = new Plane();
		buildingPlane.Set3Points(
			g.GetPosFromVertexID(triangle[0]),
			g.GetPosFromVertexID(triangle[1]),
			g.GetPosFromVertexID(triangle[2])
		);
		Vector3 normal = buildingPlane.normal;

		// average over the corners
		Vector3 centroid =
			triangle.Select(id => g.GetPosFromVertexID(id)).Aggregate(new Vector3(), (acc, x) => acc + x) / triangle.Length;

		int newVertexId = g.vertices.Length;
		Vertex newVertex = new Vertex(newVertexId, normal * distanceFromTriangle + centroid,false);

		int newEdgeIdStart = g.edges.Length;
		List<Edge> newEdges = Enumerable.Range(0, 3).Select(i => new Edge(newEdgeIdStart + i, newVertexId, triangle[i],false)).ToList();

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
		TriangleSelection.addGeometryEvent.AddListener(AddGeometry);

		// initialize Model with some dummy data (tetrahedron)
		this.graph = Graph.InitTetrahedron();
		modelUpdate.Invoke();
	}

	public Graph getGraph()
	{
		return graph;
	}

	public void AddGeometry(int[] triangle)
	{
		this.graph = Graph.AddGeometry(graph, triangle);
		modelUpdate.Invoke();
	}
}