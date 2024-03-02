using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public enum Axel { Front, Rear }

    [System.Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    [SerializeField]
    private float maxAcceleration = 80000f;
    [SerializeField]
    private float brakeAcceleration = 100000f;

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

    [SerializeField]
    private Material renderTextureMaterial;

    private void Start()
    {
        carRigidBody = GetComponent<Rigidbody>();
        carRigidBody.centerOfMass = _centerOfMass;
        GameObject renderOutputObj = GameObject.FindWithTag("RenderOutput");
        if (renderOutputObj)
        {
            renderTextureMaterial = renderOutputObj.GetComponent<RawImage>().material;
        }
    }

    private void Update()
    {
        GetInputs();
        AnimateWheels();
    }

    private void LateUpdate()
    {
        Move();
        if (renderTextureMaterial)
            renderTextureMaterial.SetFloat("_CarSpeed", carRigidBody.velocity.magnitude);
    }

    private void GetInputs()
    {
        moveInput = Input.GetAxis(verticalAxis);
        steerInput = Input.GetAxis(horizontalAxis);
    }

    // TODO: remove magic numbers
    private void Move()
    {
        foreach (var wheel in wheelList)
        {
            wheel.wheelCollider.motorTorque = moveInput * maxAcceleration * Time.deltaTime;

            // Steer
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, steerLerpInterpolationValue);
            }

            // Brake
            if (Input.GetKey(KeyCode.Space))
            {
                wheel.wheelCollider.brakeTorque = brakeAcceleration * Time.deltaTime;
            }
            else
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    private void AnimateWheels()
    {
        foreach (var wheel in wheelList)
        {
            wheel.wheelCollider.GetWorldPose(out Vector3 _position, out Quaternion _rotation);
            wheel.wheelModel.transform.SetPositionAndRotation(_position, _rotation);
        }
    }
}
