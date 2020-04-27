using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class ReachTargetAgent_4 : ReachTargetAgent
{   
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 dirToTarget = (target.position - this.transform.position).normalized;

        // Target position in agent frame
        sensor.AddObservation(
            this.transform.InverseTransformPoint(target.transform.position)); // vec 3

        // Agent velocity in agent frame
        sensor.AddObservation(
            this.transform.InverseTransformVector(AgentVelocity)); // vec 3

        // Direction to target in agent frame
        sensor.AddObservation(
            this.transform.InverseTransformDirection(dirToTarget)); // vec 3
    }
}
