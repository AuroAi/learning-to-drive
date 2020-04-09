using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class RollerAgent : Agent
{
    Rigidbody rbody;
    public Transform target;
    public float speed = 10;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if (this.transform.position.y < 0)
        {
            this.rbody.angularVelocity = Vector3.zero;
            this.rbody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0);
        }

        target.position = new Vector3(Random.Range(-4, 4),
                                      0.5f,
                                      Random.Range(-4, 4));
    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
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

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        this.rbody.AddForce(controlSignal * speed);

        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  target.position);

        if (distanceToTarget < 1)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if (this.transform.position.y < 0)
        {
            EndEpisode();
        }
    }
}
