using Unity.VisualScripting;
using UnityEngine;

public class RoadController : MonoBehaviour
{
    // reference to the WaypointController script on the WaypointManager GameObject
    [SerializeField]
    private WaypointController wpControllerScript;

    void Start()
    {
        wpControllerScript = GameObject.FindGameObjectWithTag("WaypointManager").GetComponent<WaypointController>();
        if (wpControllerScript == null)
        {
            Debug.LogError("Unable to retrieve WaypointManager WaypointController script");
        }
        else
        {
            // if this road controller doesn't have the InstantiatedInInspector (meaning it was instantiated by an Instantiate call), we can tell the WaypointManager to add our wp. Otherwise, it's handled by the manager on the scene's Startup
            if (!this.CompareTag("InstantiatedInInspector"))
            {
                // pass our waypoints to WaypointController
                // getting the container first so we preserve the waypoint order
                foreach (Transform child in transform)
                {
                    // Need to add waypoints of any prehexisting tiles on the scene manually in order to preserve the waypoint order for the pathfinding algorithm
                    if (child.tag == "WaypointContainer")
                    {
                        foreach (Transform wp in child.transform)
                        {
                            wpControllerScript.AddWaypoint(wp);
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform != null)
        {
            transform.position += Vector3.back * Time.deltaTime;
        }
        else
        {
            // Handle the case where the transform is null, for example:
            Debug.LogWarning("Transform is null!");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy"))
        {
            // Destroy the GameObject this script is attached to
            Destroy(gameObject);
        }
    }
}
