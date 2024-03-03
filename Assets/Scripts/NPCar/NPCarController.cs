using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCarController : MonoBehaviour
{
    private Queue<Transform> waypoints;

    [SerializeField]
    private float baseSpeed = 10.0f;
    [SerializeField]
    private float acceleration = 5.0f;
    private Vector3 nextWaypointPos; // checkpoint we are navigating to

    private float DEG_TO_RAD = Mathf.PI / 180;

    // Start is called before the first frame update
    void Start()
    {
        nextWaypointPos = Vector3.zero;
        waypoints = new Queue<Transform>();
        // initialize waypoints from tiles already present when the NPCar is instantiated
        // iterating through containers, then through their children, to make sure we get the order right
        //      Note: Need to go all the way from the Tiles container to the children to make sure we get the order right, as unity's find methods don't guarantee any order.
        // There's also no need to update the checkpoints, as any additional checkpoint that might be created comes with tiles the NPCar can never travel to
        GameObject tilesParent = GameObject.FindGameObjectWithTag("TilesParent");
        if (!tilesParent)
        {
            Debug.LogError("Unable to get tiles parent object");
            return;
        }

        Transform prevWaypoint = null;  // since Peeking a queue gives us the next item to dequeue, not the last enqueued item, we need to store the previously enqueued element

        // iterate through all tiles in reverse
        for (int i = tilesParent.transform.childCount - 1; i >= 0; i--)
        {
            Transform tile = tilesParent.transform.GetChild(i);
            // iterate through all tile components
            foreach (Transform child in tile)
            {
                // find waypoint container
                if (child.tag == "WaypointContainer")
                {
                    Transform container = child;
                    // this should also theoretically work with a stack, and going up in indices instead of down
                    // but I am way too tired to make it work, and this works so let's keep it that way :)
                    for (int j = container.transform.childCount - 1; j >= 0; j--)
                    {
                        // ignore waypoints behind where the car was instantiated
                        if (container.transform.GetChild(j).transform.position.z > transform.position.z)
                        {
                            continue;
                        }
                        // can't peek an empty queue duh
                        if (waypoints.Count == 0)
                        {
                            waypoints.Enqueue(container.transform.GetChild(j));
                            prevWaypoint = container.transform.GetChild(j);
                            continue;
                        }

                        // waypoints overlap at the start and end of the tiles, so we need to filter them out, and only keep the non-overlapping ones (usually filter the last waypoint of each tile)
                        Transform currWaypoint = container.transform.GetChild(j);
                        if (prevWaypoint.transform.position != currWaypoint.transform.position)
                        {
                            waypoints.Enqueue(container.transform.GetChild(j));
                            prevWaypoint = container.transform.GetChild(j);
                        }
                    }
                }
            }
        }
        // PrintQ(); // for debugging purposes
        nextWaypointPos = waypoints.Dequeue().position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        baseSpeed += acceleration * Time.fixedDeltaTime;
        transform.position += transform.forward * baseSpeed * Time.fixedDeltaTime;
    }

    // was pretty useful for debugging
    void PrintQ()
    {
        Queue<Transform> test = waypoints;
        String queueContents = "";
        foreach (Transform t in test)
        {
            queueContents += t.name + ", ";
        }
        Debug.Log(queueContents);
    }

    private void OnTriggerEnter(Collider other)
    {
        // hit a waypoint
        if (other.gameObject.CompareTag("Waypoint"))
        {
            // only do stuff when triggering for the first time with a waypoint with the next intended position
            // if we've already hit one of the overlapping triggers once, we've already dequeued and obtained the next position
            if (other.transform.position == nextWaypointPos)
            {
                // all waypoints were passed
                if (waypoints.Count == 0)
                {
                    return;
                }
                nextWaypointPos = waypoints.Dequeue().position;

                // calculate angle to next waypoint pos
                Vector3 dirToNextWaypoint = new Vector3(nextWaypointPos.x, 0.0f, nextWaypointPos.z) - new Vector3(transform.position.x, 0.0f, transform.position.z); // ignoring y coordinate variation
                float angle = Vector3.Angle(dirToNextWaypoint, transform.forward);
                float angleRad = angle * DEG_TO_RAD;

                // between a certain threshold (around 75º), we consider the NPCar needs to turn
                if (Mathf.Cos(angleRad) <= 0.25)
                {
                    Quaternion rotation = Quaternion.LookRotation(nextWaypointPos - transform.position);
                    transform.rotation = rotation;
                }
            }
        }
    }
}
