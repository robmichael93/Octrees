using UnityEngine;

public class CreateOctree : MonoBehaviour
{
    public GameObject[] worldObjects;
    public int nodeMinSize = 5;
    Octree ot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ot = new Octree(worldObjects, nodeMinSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            ot.rootNode.Draw();
        }
    }
}
