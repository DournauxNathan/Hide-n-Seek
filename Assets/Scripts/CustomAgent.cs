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

    [SerializeField] private GameObject env;

    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    
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
        hidingSpot.Reset();
        transform.localPosition = new Vector3(-2.72f, 0, 0);
        //transform.localPosition = new Vector3(UnityEngine.Random.Range(-4f, 0.75f), 0, UnityEngine.Random.Range(-2f, +2f));
        //hidingSpot.transform.localPosition = new Vector3(UnityEngine.Random.Range(2.4f, +4f), .5f, UnityEngine.Random.Range(-2f, +2f));

        //OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty);

        // Reset the environment for a new episode, if needed.
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Define the agent's observations. These are the state variables for the agent.
        sensor.AddObservation(transform.position);
        sensor.AddObservation(hidingSpot.transform.position);


        /*// Check if the hidingSpot is already taken
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
        }*/
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0]; // 0 = Don't Move; 1 = Left; 2 = Right
        float moveZ = actions.ContinuousActions[1]; // 0 = Don't Move; 1 = Back; 2 = Forward

        float moveSpeed = 5f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<HidingSpot>(out HidingSpot spot))
            {
                if (spot.CanHide())
                {
                    SetReward(+1f);
                    spot.Taken();
                    floorMeshRenderer.material = winMaterial;
                    EndEpisode();
                }
            }
        }

        /*Vector3 addForce = new Vector3(moveX, 0, moveZ);

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


        agentRigidbody.velocity = addForce * moveSpeed + new Vector3(0, agentRigidbody.velocity.y, 0);

        
        */
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal"); 
        continuousAction[1] = Input.GetAxisRaw("Vertical");
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
