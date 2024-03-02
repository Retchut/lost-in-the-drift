using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAroundOrigin : MonoBehaviour
{
    public float height = 2.5f;
    public float radius = 3f;
    public float speed = 1f;

    private void Update()
    {
        // Calculate the new position based on the time
        float time = Time.time * speed;

        // Set the new position of the camera
        transform.position = new Vector3(Mathf.Sin(time) * radius, height, Mathf.Cos(time) * radius);
        transform.LookAt(Vector3.zero);
    }
}
