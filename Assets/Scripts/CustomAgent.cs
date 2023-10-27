using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CustomAgent : Agent
{
    public event EventHandler onHidingSpotDetected;
    public event EventHandler OnEpisodeBeginEvent;

    [SerializeField] private HidingSpot hidingSpot;
    
    private Rigidbody agentRigidbody;

    private void Awake()
    {
        agentRigidbody = GetComponent<Rigidbody>();
    }

    public override void Initialize()
    {
        // Initialize your agent, e.g., set up references or perform any necessary setup.
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;

        OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty);

        // Reset the environment for a new episode, if needed.
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Define the agent's observations. These are the state variables for the agent.

        // Check if the hidingSpot is already taken
        sensor.AddObservation(hidingSpot.CanHide() ? 1 : 0);
        
        sensor.AddObservation(GameManager.instance.HasGameBegin() ? 1 : 0);

        if (!GameManager.instance.HasGameBegin())
        {
            // Calculte the direction between the hiding spot and the agent
            Vector3 dirToSpot = (hidingSpot.transform.localPosition - transform.position).normalized;
            sensor.AddObservation(dirToSpot.x);
            sensor.AddObservation(dirToSpot.z);
        }
        else
        {
            sensor.AddObservation(0f); // x
            sensor.AddObservation(0f); // z
        }    
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveX = actions.DiscreteActions[0]; // 0 = Don't Move; 1 = Left; 2 = Right
        int moveZ = actions.DiscreteActions[0]; // 0 = Don't Move; 1 = Back; 2 = Forward

        Vector3 addForce = new Vector3(moveX, 0, moveZ);

        switch (moveX)
        {
            case 0: addForce.x = 0f; break;
            case 1: addForce.x = -1f; break;
            case 2: addForce.x = +1f; break;
        }

        switch (moveZ)
        {
            case 0: addForce.z = 0f; break;
            case 1: addForce.z = -1f; break;
            case 2: addForce.z = +1f; break;
        }


        float moveSpeed = 5f;
        agentRigidbody.velocity = addForce * moveSpeed + new Vector3(0, agentRigidbody.velocity.y, 0);

        bool isHidingSpotTaken = actions.DiscreteActions[2] == 1;
        if (isHidingSpotTaken)
        {
            Collider[] colliderArray = Physics.OverlapBox(transform.position, Vector3.up * .5f);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<HidingSpot>(out HidingSpot spot))
                {
                    if (spot.CanHide())
                    {
                        spot.Taken();
                        AddReward(1f);
                    }
                }
            }
        }

        AddReward(-1f / MaxStep);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<HidingSpot>(out HidingSpot spot))
        {
            AddReward(1f);
            onHidingSpotDetected.Invoke(this, EventArgs.Empty);

            EndEpisode();
        }
    }
}
