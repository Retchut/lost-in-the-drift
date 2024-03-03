using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCarController : MonoBehaviour
{
    [SerializeField]
    private List<Transform> wpPositions;

    private float baseSpeed = 10.0f;
    private float acceleration = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        wpPositions = new List<Transform>();
        // initialize waypoints from tiles already present when the NPCar is instantiated
        // iterating through containers, then through their children, to make sure we get the order right
        //      Note: the FindBlaBlaBla methods iterate from the top of the hierarchy down. So tiles lower down in the inspector were instantiated later!
        GameObject[] wpContainers = GameObject.FindGameObjectsWithTag("WaypointContainer");
        foreach (GameObject container in wpContainers)
        {
            foreach (Transform wp in container.transform)
            {
                Debug.Log(wp.transform.position);
                wpPositions.Add(wp.transform);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        baseSpeed += acceleration * Time.fixedDeltaTime;
        transform.position += transform.forward * baseSpeed * Time.fixedDeltaTime;
    }
}
