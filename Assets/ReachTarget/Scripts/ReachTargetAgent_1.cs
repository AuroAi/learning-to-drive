using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class ReachTargetAgent_1 : ReachTargetAgent
{
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and agent positions
        sensor.AddObservation(target.position); // vec 3
        sensor.AddObservation(this.transform.position); // vec 3

        // Agent velocity
        sensor.AddObservation(AgentVelocity); // vec 3

        // The direction from the agent to the target
        Vector3 dirToTarget = (target.position - this.transform.position).normalized;
        // The alignment of the agent's velocity with this direction
        float velocityAlignment = Vector3.Dot(dirToTarget, AgentVelocity);

        // Small reward for moving in the direction of the target
        AddReward(RewardScalar * velocityAlignment);
    }
}
