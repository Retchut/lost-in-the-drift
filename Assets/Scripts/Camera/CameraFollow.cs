using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float movementSmoothness;
    public float rotationSmoothness; 

    public Vector3 movementOffset;
    public Vector3 rotationOffset;
    private Vector3 targetPosition;

    public Transform target; 

    private void FixedUpdate() {
        FollowTarget();
    }

    private void FollowTarget() {
        // Movement
        targetPosition = target.TransformPoint(movementOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, movementSmoothness * Time.deltaTime);
    
        // Rotation
        var _direction = target.position - transform.position;
        var _rotation = Quaternion.LookRotation(_direction + rotationOffset, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, rotationSmoothness * Time.deltaTime);
    }
}
