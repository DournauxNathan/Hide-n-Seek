using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CustomAgent : Agent
{
    [Header("Refs")]
    [SerializeField] private HidingSpot hidingSpot;
    [SerializeField] private GameObject env;

    [Header("Hiding Time")]
    [SerializeField] private float timer;
    [SerializeField] private bool runTimer; 

    [Header("End Episode Visualizer")]
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private Material wallCollisionMaterial;
    [Header("End Episode Visualizer")]
    
    Rigidbody m_AgentRb;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();


        m_AgentRb.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0, 360)));

        timer = GameManager.instance.hidingTime;
        runTimer = true;
    }

    public override void OnEpisodeBegin()
    {
        hidingSpot.Reset();

        transform.localPosition = Vector3.zero;

        //Reset Timer
        timer = GameManager.instance.hidingTime;
        runTimer = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Define the agent's observations. These are the state variables for the agent.
        sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));

        sensor.AddObservation(hidingSpot.transform.position);

        sensor.AddObservation(timer);
    }

    public void DetectHidingSpot()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 1f);
        
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<HidingSpot>(out HidingSpot spot))
            {
                if (spot.CanHide() && timer > 0)
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
        //AddReward(-1f / MaxStep);

        if (runTimer)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                runTimer = false;
                SetReward(-1f);
                floorMeshRenderer.material = loseMaterial;
                EndEpisode();
            }
        }

        MoveAgent(actions.DiscreteActions);
        DetectHidingSpot();
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

        //transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * .5f, ForceMode.VelocityChange);
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            floorMeshRenderer.material = wallCollisionMaterial;
            EndEpisode();
        }
    }
}
