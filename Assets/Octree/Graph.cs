using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public List<Edge> edges = new List<Edge>();
    public List<Node> nodes = new List<Node>();
    List<Node> pathList = new List<Node>();

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

    public int getPathLength()
    {
        return pathList.Count;
    }

    public OctreeNode getPathPoint(int index)
    {
        return pathList[index].octreeNode;
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

    public bool AStar(OctreeNode startNode, OctreeNode endNode)
    {
        pathList.Clear();
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

            open.Remove(current);
            closed.Add(current);
            List<Edge> currentEdges = current.edgeList;
            for (int j = 0; j < currentEdges.Count; j++)
            {
                // Reset the score & flag for each neighbor
                tentative_g_score = 0.0f;
                tentative_is_better = false;
                
                // Check if one of the neighbors is the goal location
                if (currentEdges[j].endNode.getNode().id == endNode.id)
                {
                    closed.Add(currentEdges[j].endNode);
                    open.Clear();
                    pathList = closed;
                    return true;
                }

                // Otherwise, keep searching the neighboring nodes
                // Skip any node already in the closed list, as this is the path being built
                if (!closed.Contains(currentEdges[j].endNode))
                {
                    // Add any neighboring node not on the open list, and calculate its h, g, & f score, then set its parent (cameFrom) to the current if it doesn't have
                    // a parent
                    if (!open.Contains(currentEdges[j].endNode))
                    {
                        open.Add(currentEdges[j].endNode);
                        currentEdges[j].endNode.h = Vector3.SqrMagnitude(currentEdges[j].endNode.getNode().getNodeBounds().center - endNode.getNodeBounds().center);
                        currentEdges[j].endNode.g = current.g + Vector3.SqrMagnitude(current.getNode().getNodeBounds().center - 
                                                    currentEdges[j].endNode.getNode().getNodeBounds().center);
                        currentEdges[j].endNode.f = currentEdges[j].endNode.g + currentEdges[j].endNode.h;
                        if (currentEdges[j].endNode.cameFrom == null)
                        {
                            currentEdges[j].endNode.cameFrom = current;
                        }
                    }
                    else // For any neighbors already in the open list, recalculate the tentative g score to see if it's better than what it already has
                    {
                        tentative_g_score = current.g + Vector3.SqrMagnitude(current.getNode().getNodeBounds().center - 
                                                                             currentEdges[j].endNode.getNode().getNodeBounds().center);
                        tentative_is_better = tentative_g_score < currentEdges[j].endNode.g;
                    }
                    // If the g score is better, give it that score instead of what it had, updated the f score, and change its parent (cameFrom) to current
                    if (tentative_is_better)
                    {
                        currentEdges[j].endNode.g = tentative_g_score;
                        currentEdges[j].endNode.f = currentEdges[j].endNode.g + currentEdges[j].endNode.h;
                        currentEdges[j].endNode.cameFrom = current;
                    }          
                }
            }
        }
        return false;
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
