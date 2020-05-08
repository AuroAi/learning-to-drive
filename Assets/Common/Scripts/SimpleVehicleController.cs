using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVehicleController : MonoBehaviour
{
    public float maxSteeringAngle;
    public float maxMotorTorque;
    public float maxBrakeTorque;
    public AxleInfo[] axleInfos;
    public WheelInfo wheelInfo;

    private float m_steering = 0;
    private float m_throttle = 0;
    private float m_brake = 0;

    public void SetSteering(float value)
    {
        m_steering = Mathf.Clamp(value, -1, 1);
    }

    public void SetThrottle(float value)
    {
        m_throttle = Mathf.Clamp(value, -1, 1);
    }

    public void SetBrake(float value)
    {
        m_brake = Mathf.Clamp(value, 0, 1);
    }

    void Awake()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            CreateWheelCollider(axleInfo.leftWheel);
            CreateWheelCollider(axleInfo.rightWheel);
        }
    }

    public void FixedUpdate()
    {
        float steerAngle = maxSteeringAngle * m_steering;
        float motorTorque = maxMotorTorque * m_throttle;
        float brakeTorque = maxBrakeTorque * m_brake;

        foreach (var axleInfo in axleInfos)
        {
            var wcLeft = axleInfo.leftWheel.GetComponent<WheelCollider>();
            var wcRight = axleInfo.rightWheel.GetComponent<WheelCollider>();

            if (axleInfo.steering)
            {
                wcLeft.steerAngle = steerAngle;
                wcRight.steerAngle = steerAngle;
            }
            if (axleInfo.motor)
            {
                wcLeft.motorTorque = motorTorque;
                wcRight.motorTorque = motorTorque;
            }
            if (axleInfo.brake)
            {
                wcLeft.brakeTorque = brakeTorque;
                wcRight.brakeTorque = brakeTorque;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(GameObject wheel)
    {
        if (wheel.transform.GetChild(0) == null)
            return;

        Vector3 position;
        Quaternion rotation;
        wheel.GetComponent<WheelCollider>()
             .GetWorldPose(out position, out rotation);

        wheel.transform.GetChild(0).position = position;
        wheel.transform.GetChild(0).rotation = rotation;
    }

    void CreateWheelCollider(GameObject wheel)
    {
        // if wheel collider already exists, exit
        if (wheel.GetComponent<WheelCollider>())
        {
            return;
        }

        WheelCollider wc = wheel.AddComponent<WheelCollider>();
        wc.radius = wheelInfo.wheelRadius;
        wc.suspensionDistance = wheelInfo.suspensionDistance;

        WheelFrictionCurve fwdFriction = new WheelFrictionCurve();
        fwdFriction.extremumSlip = wheelInfo.forwardExtremumSlip;
        fwdFriction.extremumValue = wheelInfo.forwardExtremumValue;
        fwdFriction.asymptoteSlip = wheelInfo.forwardAsymptoteSlip;
        fwdFriction.asymptoteValue = wheelInfo.forwardAsymptoteValue;
        fwdFriction.stiffness = wheelInfo.forwardStiffness;
        wc.forwardFriction = fwdFriction;

        WheelFrictionCurve sideFriction = new WheelFrictionCurve();
        sideFriction.extremumSlip = wheelInfo.sidewaysExtremumSlip;
        sideFriction.extremumValue = wheelInfo.sidewaysExtremumValue;
        sideFriction.asymptoteSlip = wheelInfo.sidewaysAsymptoteSlip;
        sideFriction.asymptoteValue = wheelInfo.sidewaysAsymptoteValue;
        sideFriction.stiffness = wheelInfo.sidewaysStiffness;
        wc.sidewaysFriction = sideFriction;

        JointSpring suspSpring = new JointSpring();
        suspSpring.spring = wheelInfo.suspensionSpring;
        suspSpring.damper = wheelInfo.suspensionDamper;
        wc.suspensionSpring = suspSpring;
    }
}

[System.Serializable]
public class AxleInfo
{
    public GameObject leftWheel;
    public GameObject rightWheel;
    public bool motor;
    public bool steering;
    public bool brake;
}

[System.Serializable]
public class WheelInfo
{
    public float wheelRadius = 0.5f;
    public float suspensionDistance = 0.1f;
    [Header("Suspension Spring")]
    public float suspensionSpring = 35000f;
    public float suspensionDamper = 4500f;
    [Header("Forward Friction")]
    public float forwardExtremumSlip = 0.4f;
    public float forwardExtremumValue = 1f;
    public float forwardAsymptoteSlip = 0.8f;
    public float forwardAsymptoteValue = 0.5f;
    public float forwardStiffness = 2f;
    [Header("Sideways Friction")]
    public float sidewaysExtremumSlip = 0.2f;
    public float sidewaysExtremumValue = 1f;
    public float sidewaysAsymptoteSlip = 0.8f;
    public float sidewaysAsymptoteValue = 0.5f;
    public float sidewaysStiffness = 1f;
}
