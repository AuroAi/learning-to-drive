using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class ReachTargetAgent_0 : ReachTargetAgent
{
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and agent positions
        sensor.AddObservation(target.position); // vec 3
        sensor.AddObservation(this.transform.position); // vec 3

        // Agent velocity
        sensor.AddObservation(AgentVelocity); // vec 3
    }
}
