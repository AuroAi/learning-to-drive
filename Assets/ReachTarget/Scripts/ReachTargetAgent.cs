using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SimpleVehicleController))]
public class ReachTargetAgent : Agent
{
    public Transform target;
    public Renderer ground;
    private Rigidbody rbody;
    private SimpleVehicleController controller;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        controller = GetComponent<SimpleVehicleController>();

    }

    public override void OnEpisodeBegin()
    {
        ResetTarget();
    }

    public override float[] Heuristic()
    {
        var action = new float[3];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        action[2] = Input.GetAxis("Jump");
        return action;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and agent positions
        sensor.AddObservation(target.position);
        sensor.AddObservation(this.transform.position);

        // Agent velocity
        sensor.AddObservation(this.rbody.velocity.x);
        sensor.AddObservation(this.rbody.velocity.z);
    }

    public override void OnActionReceived(float[] action)
    {
        controller.Steering = action[0];
        controller.Throttle = action[1];
        controller.Brake = action[2];

        var distanceToTarget = Vector3.Distance(
            this.transform.position,
            target.transform.position
        );

        if (distanceToTarget < 1)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if (this.transform.localPosition.y < 0)
        {
            ResetAgent();
            EndEpisode();
        }
    }

    void ResetAgent()
    {
        this.controller.Throttle = 0;
        this.controller.Steering = 0;
        this.controller.Brake = 1;
        this.rbody.angularVelocity = Vector3.zero;
        this.rbody.velocity = Vector3.zero;
        this.rbody.angularVelocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 1f, 0);
        this.transform.rotation = Quaternion.Euler(0, Random.Range(-180, 180), 0);
    }

    void ResetTarget()
    {
        Vector3 extents = ground.bounds.extents - (Vector3.one * 3);
        target.localPosition = new Vector3(Random.Range(-extents.x, extents.x),
                                           0.5f,
                                           Random.Range(-extents.z, extents.z));
    }

    void OnCollisionEnter(Collision collision)
    {
        ResetAgent();
        EndEpisode();
    }
}
