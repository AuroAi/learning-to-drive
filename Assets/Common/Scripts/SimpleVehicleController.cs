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

    private float steering = 0;
    private float throttle = 0;
    private float brake = 0;

    public float Steering
    {
        get { return steering; }
        set { steering = Mathf.Clamp(value, -1, 1); }
    }

    public float Throttle
    {
        get { return throttle; }
        set { throttle = Mathf.Clamp(value, -1, 1); }
    }

    public float Brake
    {
        get { return brake; }
        set { brake = Mathf.Clamp(value, 0, 1); }
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
        float motor = maxMotorTorque * this.throttle;
        float steering = maxSteeringAngle * this.steering;
        float brake = maxBrakeTorque * this.brake;
     
        foreach (var axleInfo in axleInfos) {
            var wcLeft = axleInfo.leftWheel.GetComponent<WheelCollider>();
            var wcRight = axleInfo.rightWheel.GetComponent<WheelCollider>();

            if (axleInfo.steering) {
                wcLeft.steerAngle = steering;
                wcRight.steerAngle = steering;
            }
            if (axleInfo.motor) {
                wcLeft.motorTorque = motor;
                wcRight.motorTorque = motor;
            }
            if (axleInfo.brake) {
                wcLeft.brakeTorque = brake;
                wcRight.brakeTorque = brake;
            }
            ApplyLocalPositionToVisuals(wcLeft);
            ApplyLocalPositionToVisuals(wcRight);
        }
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        Transform visualWheel = collider.transform.GetChild(0);
        visualWheel.position = position;
        visualWheel.rotation = rotation;
    }

    void CreateWheelCollider(GameObject wheel)
    {
        // if wheel collider already exists, exit
        if (wheel.GetComponent<WheelCollider>())
        {
            return;
        }

        WheelCollider wc = wheel.AddComponent<WheelCollider>();
        float refRadius = 1;
        if (wheel.GetComponentInChildren<Renderer>() != null)
        {
            var visualWheelBounds = wheel.GetComponentInChildren<Renderer>().bounds;
            refRadius = visualWheelBounds.extents.magnitude / 2;
        }

        wc.radius = wheelInfo.wheelScale * refRadius;
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
public class AxleInfo {
    public GameObject leftWheel;
    public GameObject rightWheel;
    public bool motor;
    public bool steering;
    public bool brake;
}

[System.Serializable]
public class WheelInfo
{
    public float wheelScale = 1f;
    public float suspensionDistance = 0.1f;
    public float suspensionSpring = 35000f;
    public float suspensionDamper = 4500f;
    public float forwardExtremumSlip = 0.4f;
    public float forwardExtremumValue = 1f;
    public float forwardAsymptoteSlip = 0.8f;
    public float forwardAsymptoteValue = 0.5f;
    public float forwardStiffness = 2f;
    [Space(10)]
    public float sidewaysExtremumSlip = 0.2f;
    public float sidewaysExtremumValue = 1f;
    public float sidewaysAsymptoteSlip = 0.8f;
    public float sidewaysAsymptoteValue = 0.5f;
    public float sidewaysStiffness = 1f;
}
