using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SimpleVehicleController))]
public class ReachTargetAgent_3 : Agent
{
    public Transform target;
    public Renderer ground;
    private Rigidbody m_rbody;
    private SimpleVehicleController m_controller;

    void Start()
    {
        m_rbody = GetComponent<Rigidbody>();
        m_controller = GetComponent<SimpleVehicleController>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Direction to target and target position in agent frame
        Vector3 dirToTarget = (target.position - gameObject.transform.position).normalized;
        sensor.AddObservation(gameObject.transform.InverseTransformDirection(dirToTarget)); // vec 3
        sensor.AddObservation(gameObject.transform.InverseTransformPoint(target.transform.position)); // vec 3

        // Agent velocity in agent frame
        sensor.AddObservation(gameObject.transform.InverseTransformVector(m_rbody.velocity)); // vec 3

        // Small reward for moving in the direction of the target
        float velocityAlignment = Vector3.Dot(dirToTarget, m_rbody.velocity);
        float scalar = (maxStep > 0) ? 1f / (float)maxStep : 1E-4f;
        AddReward(scalar * velocityAlignment);

        var distanceToTarget = Vector3.Distance(
            gameObject.transform.position,
            target.transform.position
        );

        // Upon reaching the target, respawn it to a random position
        // and add reward of +1
        if (distanceToTarget < 1)
        {
            AddReward(1f);
            RespawnTarget();
        }

        // If the car falls off the platform, end episode
        if (gameObject.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        RespawnTarget();
        RespawnAgent();
    }

    public override void OnActionReceived(float[] action)
    {
        m_controller.Steering = action[0];
        m_controller.Throttle = action[1];
        m_controller.Brake = action[2];
    }

    void OnCollisionEnter(Collision collision)
    {
        EndEpisode();
    }

    public override float[] Heuristic()
    {
        var action = new float[3];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        action[2] = Input.GetAxis("Jump");
        return action;
    }

    void RespawnAgent()
    {
        m_controller.Throttle = 0;
        m_controller.Steering = 0;
        m_controller.Brake = 1;
        m_rbody.velocity = Vector3.zero;
        m_rbody.angularVelocity = Vector3.zero;
        gameObject.transform.localPosition = new Vector3(0, 1f, 0);
        gameObject.transform.rotation = Quaternion.Euler(0, Random.Range(-180, 180), 0);
    }

    void RespawnTarget()
    {
        Vector3 extents = ground.bounds.extents - (Vector3.one * 3);
        target.localPosition = new Vector3(Random.Range(-extents.x, extents.x),
                                           0.5f,
                                           Random.Range(-extents.z, extents.z));
        target.rotation = Quaternion.Euler(0, Random.Range(-180, 180), 0);
    }
}
