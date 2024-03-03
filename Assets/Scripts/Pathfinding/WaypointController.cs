using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour
{
    [SerializeField]
    // waypoints go from start of tile to the end of the tile, sequentially
    // NPCars read waypoints in the reverse order, for their pathfinding algorithm
    private List<Transform> childWps;
    void Start()
    {
        childWps = new List<Transform>();
        // Need to add waypoints of any prehexisting tiles on the scene manually in order to preserve the waypoint order for the pathfinding algorithm
        RetrieveSceneWaypoints();
    }

    void Update()
    {
        // check for new children gizmos
    }

    public void AddWaypoint(Transform wp)
    {
        childWps.Add(wp);
    }

    private void RetrieveSceneWaypoints()
    {
        // initialize waypoints from tiles already present when the scene loads
        // iterating through containers, then through their children, to make sure we get the order right
        //      Note: the FindBlaBlaBla methods iterate from the top of the hierarchy down. So the 1st tile must be above the second!!
        GameObject[] wpContainers = GameObject.FindGameObjectsWithTag("WaypointContainer");
        foreach (GameObject container in wpContainers)
        {
            foreach (Transform child in container.transform)
            {
                AddWaypoint(child.transform);
            }
        }
    }
}
