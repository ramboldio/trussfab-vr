using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public struct Edge
{
    public readonly int id;
    public readonly int v_src;
    public readonly int v_dest;
    public bool actuator;

    public Edge(int id, int v_src, int v_dest, bool actuator)
    {
        this.id = id;
        this.v_src = v_src;
        this.v_dest = v_dest;
        this.actuator = actuator;
    }
}

public struct Vertex
{
    public readonly int id;
    public readonly Vector3 pos;
    public bool inflationVertex;

    public Vertex(int id, Vector3 pos, bool inflationVertex)
    {
        this.id = id;
        this.pos = pos;
        this.inflationVertex = inflationVertex;
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

    // Embedded dino.obj content as a verbatim string
    private static readonly string dinoObjContent = @"
v 2.405123 6.187879 -0.567431
v 4.467745 6.065675 -0.444829
v 7.023954 7.614221 -0.296009
v 7.065202 6.911689 -0.320699
v 5.144553 6.485277 0.401834
v 5.168724 6.578892 -1.204038
v 6.659563 5.194789 -0.393004
v 4.228254 7.457746 -0.893511
v 4.280571 7.396937 0.214404
v 3.619620 5.227151 0.198013
v 3.592076 5.272702 -1.062384
l 4 5
l 4 6
l 5 6
l 5 3
l 3 6
l 5 7
l 6 7
l 5 2
l 2 6
l 3 4
l 8 6
l 2 9
l 9 5
l 8 3
l 8 2
l 9 8
l 3 9
l 10 2
l 2 11
l 8 1
l 11 8
l 1 9
l 9 10
l 2 7
";

    public static Graph InitFromString()
    {
        var vertices = new List<Vertex>();
        var edges = new List<Edge>();

        int vertexId = 0;
        int edgeId = 0;

        var lines = dinoObjContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var rawLine in lines)
        {
            string line = rawLine.Trim();

            if (line.StartsWith("v "))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4)
                {
                    float x = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
                    vertices.Add(new Vertex(vertexId++, new Vector3(x, y, z), false));
                }
            }
            else if (line.StartsWith("l "))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    int v1 = int.Parse(parts[1]) - 1; // OBJ indices start at 1
                    int v2 = int.Parse(parts[2]) - 1;
                    edges.Add(new Edge(edgeId++, v1, v2, false));
                }
            }
        }

        return new Graph(vertices.ToArray(), edges.ToArray());
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

    public List<int[]> getTriangles()
    {
        List<int[]> triangles = new List<int[]>();
        bool[,] adjMatrix = getAdjecencyMatrix();
        for (int i = 0; i < this.edges.Length - 1; i++)
        {
            for (int j = i + 1; j < this.edges.Length; j++)
            {
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

    public static Graph AddGeometry(Graph g, int[] triangle)
    {
        float distanceFromTriangle = 0.5f;

        Plane buildingPlane = new Plane();
        buildingPlane.Set3Points(
            g.GetPosFromVertexID(triangle[0]),
            g.GetPosFromVertexID(triangle[1]),
            g.GetPosFromVertexID(triangle[2])
        );
        Vector3 normal = buildingPlane.normal;

        Vector3 centroid =
            triangle.Select(id => g.GetPosFromVertexID(id)).Aggregate(new Vector3(), (acc, x) => acc + x) / triangle.Length;

        int newVertexId = g.vertices.Length;
        Vertex newVertex = new Vertex(newVertexId, normal * distanceFromTriangle + centroid, false);

        int newEdgeIdStart = g.edges.Length;
        List<Edge> newEdges = Enumerable.Range(0, 3)
            .Select(i => new Edge(newEdgeIdStart + i, newVertexId, triangle[i], false))
            .ToList();

        return new Graph(
            g.vertices.Append(newVertex).ToArray(),
            g.edges.Concat(newEdges).ToArray()
        );
    }
}

public class Model : MonoBehaviour
{
    public UnityEvent modelUpdate = new UnityEvent();

    private Graph graph;

    void Start()
    {
        TriangleSelection.addGeometryEvent.AddListener(AddGeometry);
        Debug.Log("Dino view component: " + GameObject.Find("Dino").GetComponent<View>());
        
        // Initialize graph by parsing the embedded OBJ string
        this.graph = Graph.InitFromString();

        modelUpdate.Invoke();
        Debug.Log("modelUpdate invoked");
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
