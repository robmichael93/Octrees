using UnityEngine;

public class CreateOctree : MonoBehaviour
{
    public GameObject[] worldObjects;
    public int nodeMinSize = 5;
    Octree ot;
    Graph waypoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waypoints = new Graph();
        ot = new Octree(worldObjects, nodeMinSize, waypoints);
    }

    public Graph getWaypoints()
    {
        return waypoints;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            ot.rootNode.Draw();
            ot.navigationGraph.Draw();
        }
    }
}
