using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Graph
{
    public List<Edge> edges = new List<Edge>();
    public List<Node> nodes = new List<Node>();

    public Graph()
    {
    }

    public void AddNode(OctreeNode otn)
    {
        if (findNode(otn.id) == null)
        {
            Node node = new Node(otn);
            nodes.Add(node);            
        }
    }

    public void AddEdge(OctreeNode fromNode, OctreeNode toNode)
    {
        Node from = findNode(fromNode.id);
        Node to = findNode(toNode.id);
        if (from != null && to != null)
        {
            Edge e = new Edge(from, to);
            edges.Add(e);
            from.edgeList.Add(e);
            Edge f = new Edge(to, from);
            edges.Add(f);
            to.edgeList.Add(f);
        }
    }

    Node findNode(int otn_id)
    {
        foreach(Node n in nodes)
        {
            if (n.getNode().id == otn_id)
            {
                return n;
            }
        }
        return null;
    }



    public void Draw()
    {
        for (int i = 0; i < edges.Count; i++)
        {
            Debug.DrawLine(edges[i].startNode.getNode().getNodeBounds().center, edges[i].endNode.getNode().getNodeBounds().center, Color.red, 0.1f);
        }
        Gizmos.color = new Color(1, 1, 0);
        for (int j = 0; j < nodes.Count; j++)
        {
            Gizmos.DrawWireSphere(nodes[j].getNode().getNodeBounds().center, 0.25f);
        }
    }

    public bool AStar(OctreeNode startNode, OctreeNode endNode, List<Node> path)
    {
        path.Clear();
        Node start = findNode(startNode.id);
        Node end = findNode(endNode.id);

        if (start == null || end == null)
        {
            return false;
        }

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        float tentative_g_score = 0.0f;
        bool tentative_is_better = false;

        start.g = 0.0f;
        start.h = Vector3.SqrMagnitude(startNode.getNodeBounds().center - endNode.getNodeBounds().center);
        start.f = start.h;

        open.Add(start);

        while (open.Count > 0)
        {
            int i = lowestF(open);
            Node current = open[i];

            // Check if the current node is the goal location & if so, create the path, then return true
            if (current.getNode().id == endNode.id)
            {
                reconstructPath(start, end, path);
                return true;
            }

            open.RemoveAt(i);
            closed.Add(current);

            Node neighbor;
            foreach (Edge e in current.edgeList)
            {
                neighbor = e.endNode;
                // Reset the score & flag for each neighbor
                tentative_g_score = 0.0f;
                tentative_is_better = false;

                // Skip any node already in the closed list, as this is the path being built
                if (!closed.Contains(neighbor))
                {
                    tentative_g_score = current.g + Vector3.SqrMagnitude(current.getNode().getNodeBounds().center - 
                                                neighbor.getNode().getNodeBounds().center);

                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                        tentative_is_better = true;
                    }
                    else if (tentative_g_score < neighbor.g)
                    {
                        tentative_is_better = true;
                    }
                    // If the g score is better, or for new nodes, update/assign g, h, & f, as well as cameFrom
                    if (tentative_is_better)
                    {
                        neighbor.g = tentative_g_score;
                        neighbor.h = Vector3.SqrMagnitude(neighbor.getNode().getNodeBounds().center - endNode.getNodeBounds().center);
                        neighbor.f = neighbor.g + neighbor.h;
                        neighbor.cameFrom = current;
                    }          
                }
            }
        }
        return false;
    }

    public void reconstructPath(Node start, Node end, List<Node> path)
    {
        path.Clear();
        path.Add(end);
        Node p = end.cameFrom;
        while (p != start && p != null)
        {
            path.Insert(0, p);
            p = p.cameFrom;
        }
        path.Insert(0, start);
    }

    int lowestF(List<Node> l)
    {
        float lowestF = 0;
        int count = 0;
        int iteratorCount = 0;
        for (int i = 0; i < l.Count; i++)
        {
            if (i == 0)
            {
                lowestF = l[i].f;
                iteratorCount = count;
            }
            else if (l[i]. f <= lowestF)
            {
                lowestF = l[i].f;
                iteratorCount = count;
            }
            count++;
        }
        return iteratorCount;
    }
}
