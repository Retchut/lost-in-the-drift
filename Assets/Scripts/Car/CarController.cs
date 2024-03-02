using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum Axel {Front, Rear}

    [System.Serializable]
    public struct Wheel {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    private float maxAcceleration = 22000f;
    private float brakeAcceleration = 80000f;

    private float turnSensitivity = 1.0f;
    private float maxSteerAngle = 30.0f;
    private float steerLerpInterpolationValue = 0.6f;

    private float moveInput;
    private float steerInput;

    public Vector3 _centerOfMass;

    public List<Wheel> wheelList;
    private Rigidbody carRigidBody; 

    private string verticalAxis = "Vertical";
    private string horizontalAxis = "Horizontal";

    private void Start() {
        carRigidBody = GetComponent<Rigidbody>();
        carRigidBody.centerOfMass = _centerOfMass;
    }

    private void Update() {
        GetInputs();
        AnimateWheels();
    }

    private void LateUpdate() {
        Move();
    }

    private void GetInputs() {
        moveInput = Input.GetAxis(verticalAxis);
        steerInput = Input.GetAxis(horizontalAxis);
    }

    // TODO: remove magic numbers
    private void Move() {
        foreach(var wheel in wheelList) {
            wheel.wheelCollider.motorTorque = moveInput * maxAcceleration * Time.deltaTime;
            
            // Steer
            if (wheel.axel == Axel.Front) {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, steerLerpInterpolationValue);
            }

            // Brake
            if (Input.GetKey(KeyCode.Space)) {
                wheel.wheelCollider.brakeTorque = brakeAcceleration * Time.deltaTime;
            } else {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    private void AnimateWheels() {
        foreach(var wheel in wheelList) {
            wheel.wheelCollider.GetWorldPose(out Vector3 _position, out Quaternion _rotation);
            wheel.wheelModel.transform.SetPositionAndRotation(_position, _rotation);
        }
    }
}
