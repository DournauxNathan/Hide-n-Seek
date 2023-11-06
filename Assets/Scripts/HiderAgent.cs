using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class HiderAgent : Agent
{
    public float moveSpeed = 2; // Speed of agent movement.
    public float turnSpeed = 300; // Speed of agent rotation.

    [Header("References")]
    [SerializeField] private HidingSpot hidingSpot; // Reference to the hiding spot
    [SerializeField] private GameObject env; // Reference to the environment

    [Header("Hiding Time")]
    [SerializeField] private float timer; // Time for hiding
    [SerializeField] private bool runTimer; // Flag to control the timer
    [SerializeField] private bool hasFoundHidingSpot = false;
    private bool isFound = false;// Add this flag to indicate if the HiderAgent is found by the Seeker
    public bool IsFound
    {
        get { return isFound; }
    }

    [Header("End Episode Visualizer")]
    [SerializeField] private MeshRenderer floorMeshRenderer; // Visualizer for the floor
    [SerializeField] private Material winMaterial; // Material for winning state
    [SerializeField] private Material loseMaterial; // Material for losing state
    [SerializeField] private Material wallCollisionMaterial; // Material for collision with walls

    Rigidbody m_AgentRb;

    #region Initialization

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

        m_AgentRb.velocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0, 360)));

        // Reset Timer
        timer = GameManager.instance.hidingTime;
        runTimer = true;

        hasFoundHidingSpot = false;
    }

    #endregion

    #region Observations

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent's velocity and rotation
        sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));
        sensor.AddObservation(transform.localRotation);

        // Hiding spot's position
        sensor.AddObservation(hidingSpot.transform.position);

        // Timer value
        sensor.AddObservation(timer);
        sensor.AddObservation(isFound);
    }

    #endregion

    #region Hiding Spot Detection
    

    public void OnFound()
    {
        isFound = true;
    }

    public void OnNotFound()
    {
        isFound = false;
    }

    // Add this property to track whether the agent has found a hiding spot
    private bool HasFoundHidingSpot { get; set; }

    public void DetectHidingSpot()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 1f);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<HidingSpot>(out HidingSpot spot))
            {
                if (spot.CanHide() && timer > 0)
                {

                    Vector3 hidingSpotPosition = spot.transform.position;
                    Vector3 agentPosition = transform.position;

                    // Calculate the direction to the hiding spot
                    Vector3 directionToHidingSpot = hidingSpotPosition - agentPosition;
                    directionToHidingSpot.y = 0f; // Ensure it's horizontal

                    // Rotate the agent to face the hiding spot
                    transform.LookAt(hidingSpotPosition);

                    // Move the agent towards the hiding spot
                    m_AgentRb.AddForce(directionToHidingSpot.normalized * moveSpeed, ForceMode.VelocityChange);

                    spot.Taken();
                    hasFoundHidingSpot = true; // Mark that the agent has found a hiding spot
                    floorMeshRenderer.material = winMaterial;
                    SetReward(2f);
                    EndEpisode();
                }
            }
            else
            {
                hasFoundHidingSpot = false;
            }
        }
    }

    #endregion

    #region Actions and Movement

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Timer management
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

        if (HasFoundHidingSpot)
        {
            MoveAgent(actions);
        }

        DetectHidingSpot();
    }

    public void MoveAgent(ActionBuffers actionBuffers)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        var continuousActions = actionBuffers.ContinuousActions;

        var forward = Mathf.Clamp(continuousActions[0], -1f, 1f);
        var right = Mathf.Clamp(continuousActions[1], -1f, 1f);
        var rotate = Mathf.Clamp(continuousActions[2], -1f, 1f);

        dirToGo = transform.forward * forward;
        dirToGo += transform.right * right;
        rotateDir = -transform.up * rotate;

        // Apply torque for rotation
        m_AgentRb.AddTorque(transform.up * rotate * turnSpeed, ForceMode.Force);

        m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);

        if (m_AgentRb.velocity.sqrMagnitude > 25f) // slow it down
        {
            m_AgentRb.velocity *= 0.95f;
        }
    }

    #endregion

    #region Heuristic Control

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            continuousActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            continuousActionsOut[2] = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            continuousActionsOut[0] = -1;
        }
    }

    #endregion

    #region Collision Handling

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            floorMeshRenderer.material = wallCollisionMaterial;
            EndEpisode();
        }
    }

    #endregion
}
