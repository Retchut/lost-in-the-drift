using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGizmos : MonoBehaviour
{
    // runs in editor to help with visualizing waypoints
    void OnDrawGizmos()
    {
        // runs in the editor so we can't really iterate through the waypoints list
        // draw waypoint gizmos
        Gizmos.color = Color.blue;
        foreach (Transform child in transform)
        {
            // also can't check for tag validity because tag comparison isn't performed until runtime
            Gizmos.DrawWireSphere(child.position, 1.0f);
        }

        // draw line between waypoints
        Gizmos.color = Color.cyan;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            // also can't check for tag validity because tag comparison isn't performed until runtime
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
    }
}
