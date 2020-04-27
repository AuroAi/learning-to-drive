using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class ReachTargetAgent_2 : ReachTargetAgent
{
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and agent positions
        sensor.AddObservation(target.position); // vec 3
        sensor.AddObservation(this.transform.position); // vec 3

        // Agent velocity
        sensor.AddObservation(AgentVelocity); // vec 3

        // Direction to target and agent heading
        Vector3 dirToTarget = (target.position - this.transform.position).normalized;
        sensor.AddObservation(dirToTarget); // vec 3
        sensor.AddObservation(this.transform.forward); // vec 3

        // Small reward for moving in the direction of the target
        float velocityAlignment = Vector3.Dot(dirToTarget, AgentVelocity);
        AddReward(RewardScalar * velocityAlignment);
    }
}
