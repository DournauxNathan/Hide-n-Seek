using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CustomAgent : Agent
{
    [SerializeField] private HidingSpot hidingSpot;

    [SerializeField] private GameObject env;

    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;

    [SerializeField] private float maxRaycastDistance = 5f;

    Rigidbody agentRigidbody;

    public override void Initialize()
    {
        agentRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        hidingSpot.Reset();

        agentRigidbody.velocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0, 360)));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Define the agent's observations. These are the state variables for the agent.
        sensor.AddObservation(transform.InverseTransformDirection(agentRigidbody.velocity));
        
        sensor.AddObservation(hidingSpot.transform.position);        
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        agentRigidbody.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    }

    public void DetectHidingSpot()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 1f);
        
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<HidingSpot>(out HidingSpot spot))
            {
                if (spot.CanHide())
                {
                    SetReward(2f);
                    transform.LookAt(hidingSpot.transform.localEulerAngles);
                    spot.Taken();
                    floorMeshRenderer.material = winMaterial;
                    EndEpisode();
                }
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-1f / MaxStep);

        MoveAgent(actions.DiscreteActions);
        DetectHidingSpot();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }
}
