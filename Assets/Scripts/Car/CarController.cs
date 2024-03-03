using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    // shader material
    [SerializeField]
    private Material renderTextureMaterial;

    // death conditions
    private float deathTimerMax = 10.0f;
    private float deathTimer = 10.0f;
    private Coroutine deathTimerCoroutine;
    private bool deathTimerCoroutineRunning = false;
    private float defaultGammaCorrection = 1.5f;
    private CanvasGroup dangerUIGroup;
    private TextMeshProUGUI dangerCountdownTMP;
    [SerializeField]
    private Stack<int> touchingRoadTriggers;
    bool firstTrigger = true; // avoid detecting leaving the road before the triggers load in

    private void Start()
    {
        carRigidBody = GetComponent<Rigidbody>();
        carRigidBody.centerOfMass = _centerOfMass;
        GameObject renderOutputObj = GameObject.FindWithTag("RenderOutput");
        if (renderOutputObj)
        {
            renderTextureMaterial = renderOutputObj.GetComponent<RawImage>().material;
            if (renderTextureMaterial)
                renderTextureMaterial.SetFloat("_Intensity", defaultGammaCorrection); // reset gamma correction
        }
        touchingRoadTriggers = new Stack<int>();
        deathTimer = deathTimerMax;
        dangerCountdownTMP = GameObject.FindGameObjectWithTag("DangerCountdown").GetComponent<TextMeshProUGUI>();
        if (!dangerCountdownTMP)
        {
            Debug.LogError("Couldn't get danger countdown text component");
        }
        dangerUIGroup = GameObject.FindGameObjectWithTag("DangerUI").GetComponent<CanvasGroup>();
        if (!dangerUIGroup)
        {
            Debug.LogError("Couldn't get danger UI canvas groups");
        }
        else
        {
            dangerUIGroup.alpha = 0f;
        }

    }

    private void Update()
    {
        GetInputs();
        AnimateWheels();
        // I hate the way we're checking for triggers but it works fine
        // not touching any roads
        if (touchingRoadTriggers.Count == 0)
        {
            LeaveRoad();
        }
        else
        {
            ReenterRoad();
        }
        // blink screen while in danger
        if (deathTimerCoroutineRunning && !firstTrigger)
        {
            float timeIndangerNormalized = (deathTimerMax - deathTimer) / deathTimerMax;
            float interpolatedIntensity = Mathf.Lerp(defaultGammaCorrection, defaultGammaCorrection + 5.0f, timeIndangerNormalized);
            renderTextureMaterial.SetFloat("_Intensity", interpolatedIntensity);
        }
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

    private void OnTriggerEnter(Collider other)
    {
        // The car is back on top of one road again
        if (other.tag == "RoadPath")
        {
            // this is so unbelievably iffy I hate this
            touchingRoadTriggers.Push(1); // one more trigger in contact
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // The car left one road
        if (other.tag == "RoadPath")
        {
            // this is so unbelievably iffy I hate this
            touchingRoadTriggers.Pop(); // one less trigger in contact
        }
    }

    private void ReenterRoad()
    {
        if (firstTrigger)
        {
            firstTrigger = false;
        }
        if (deathTimerCoroutineRunning)
        {
            ToggleDangerUI();
            deathTimer = deathTimerMax;
            StopCoroutine(deathTimerCoroutine);
            deathTimerCoroutineRunning = false;
            // reset dark screen
            if (renderTextureMaterial)
            {
                // original should be around 1.5
                renderTextureMaterial.SetFloat("_Intensity", defaultGammaCorrection);
            }
        }
    }

    private void LeaveRoad()
    {
        if (!deathTimerCoroutineRunning)
        {
            if (!firstTrigger)
            {
                ToggleDangerUI();
                deathTimerCoroutine = StartCoroutine(DeathTimer());
                deathTimerCoroutineRunning = true;
            }
        }
    }

    private IEnumerator DeathTimer()
    {
        float step = 0.1f;
        while (deathTimer > 0f)
        {
            deathTimer -= step;
            dangerCountdownTMP.text = Math.Round(deathTimer, 2) + "";
            yield return new WaitForSeconds(step);
        }

        // Perform actions when the countdown reaches zero (player dies)
        GameOver();
    }

    private void ToggleDangerUI()
    {
        if (dangerUIGroup.alpha == 0f)
            dangerUIGroup.alpha = 1f;
        else
            dangerUIGroup.alpha = 0f;
    }

    private void GameOver()
    {
        // Add your logic for when the player dies here
        Debug.Log("game over");
    }
}
