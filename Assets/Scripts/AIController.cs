using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public List<Transform> hidingSpots;

    private LineRenderer m_LineRenderer; // The LineRenderer for FOV detection.
    private Animator m_Animator;

    [Space(10)]
    [Header("SEEKER")] 
    public float interactionRange = 2.0f; // The range at which the AI can interact with objects.
    public float moveRange = 10.0f;
    public float moveInterval = 5.0f;
    public float wanderRadius = 5.0f;
    public float chaseSpeed = 5f; // Speed at which the AI chases other agents.
    
    [Space(10)]
    [Header("HIDER")]
    public float fleeSpeed = 7f;
    public float fleeDistance = 10f;
    public Transform[] fleeLocations;
    public Transform seeker; // Reference to

    private GameObject currentHidingObject;
    private NavMeshAgent m_NavMeshAgent;
    private Vector3 randomDestination;
    private float nextMoveTime;
    [SerializeField] private bool isSeeker = false;
    [SerializeField] private float roleSwitchTimer = 10.0f;
    private bool isChasing;

    [Space(10)]
    [Header("FOV")]
    public float fieldOfView = 90f; // Field of view angle.
    public float sightDistance = 10f; // Maximum sight distance.

    [SerializeField] private List<Transform> visibleAgents = new List<Transform>();
    private Transform currentTarget; // The currently detected agent to chase.

    // Start is called before the first frame update
    private void Start()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.enabled = false; // Initially hide the LineRenderer.
        m_NavMeshAgent = GetComponent<NavMeshAgent>();

        m_Animator = gameObject.GetComponentInChildren<Animator>();

        if (isSeeker)
        {
            // Handle initialization for the seeker.
            isSeeker = true;
            isChasing = true;
        }
        else
        {
            // Handle initialization for the hider.
            
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (isSeeker && visibleAgents.Count != 0)
        {
            // Implement seeker behavior here.
            // Check if it's time to choose a new random destination.
            if (Time.time >= nextMoveTime)
            {
                SetRandomDestination();
            }

            // Move towards the random destination.
            MoveToRandomDestination();
        }
        else
        {
            // Implement hider behavior here.

            if (currentHidingObject == null)
            {
                // Find the nearest interactable object in the hiding spot list.
                currentHidingObject = FindNearestInteractableObject();
                if (currentHidingObject != null)
                {
                    // Implement logic for the hider to move the object autonomously.
                    MoveObject(currentHidingObject);
                }
            }

            // Check if it's time to choose a new random destination.
            if (Time.time >= nextMoveTime)
            {
                SetRandomDestination();
            }

            // Move towards the random destination.
            MoveToRandomDestination();
        }

        DetectOtherAI();

        if (visibleAgents.Count > 0)
        {
            currentTarget = visibleAgents[0];

            if (isSeeker)
            {
                ChaseTarget();
                // Draw a line using the LineRenderer from the AI's position to the detected agent.// Draw a line using the LineRenderer from the AI's position to the detected agent.
                m_LineRenderer.enabled = true;
                m_LineRenderer.SetPosition(0, transform.position);
                m_LineRenderer.SetPosition(1, visibleAgents[0].position + new Vector3(0, 1.5f, 0)); // You can update this for multiple detected agents.

            }
            else
            {
                //FleeFromSeeker();
            }
        }
        else
        {
            // Draw a line using the LineRenderer from the AI's position to the detected agent.
            m_LineRenderer.enabled = false;
            currentTarget = null;
        }

        roleSwitchTimer -= Time.deltaTime;
        if (roleSwitchTimer <= 0)
        {
            Debug.Log("Switch");
            //SwitchRoles();
        }
    }

    // You can use a method to switch roles between rounds.
    private void SwitchRoles()
    {
        // Toggle between seeker and hider roles.
        isSeeker = !isSeeker;
        roleSwitchTimer = 10.0f; // Reset the role switch timer.

        // Implement logic to reset or reposition the characters for the new roles.
        if (isSeeker)
        {
            // Handle initialization for the new seeker role.
        }
        else
        {
            // Handle initialization for the new hider role.

        }
    }
    
    #region Move

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        randomDestination = hit.position;
        nextMoveTime = Time.time + moveInterval;
        m_NavMeshAgent.SetDestination(randomDestination);
    }

    private void MoveToRandomDestination()
    {
        if (!m_NavMeshAgent.pathPending && m_NavMeshAgent.remainingDistance < 0.1f)
        {
            SetRandomDestination();
        }
/*
        m_Animator.SetFloat("MovementX", randomDestination.x);
        m_Animator.SetFloat("MovementZ", randomDestination.z);*/
    }
    #endregion

    #region Interaction W/ Objects

    private GameObject FindNearestInteractableObject()
    {
        GameObject nearestObject = null;
        float closestDistance = float.MaxValue;

        foreach (Transform hidingSpot in hidingSpots)
        {
            GameObject obj = hidingSpot.gameObject;
            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (distance < closestDistance && distance <= interactionRange)
            {
                closestDistance = distance;
                nearestObject = obj;
            }
        }

        return nearestObject;
    }

    private void MoveObject(GameObject obj)
    {
        // Implement logic for the hider to move the object.
        // For example, you can use transform.Translate to move the object closer to the hider's position.
        // Be sure to check for obstacles and constraints to avoid unrealistic movements.
    }
    #endregion

    #region Detection
    private void DetectOtherAI()
    {
        visibleAgents.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, sightDistance);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("AI")) // Assuming AI agents have the "AI" tag.
            {
                Vector3 direction = collider.transform.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);

                if (angle <= fieldOfView / 2)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, direction, out hit, sightDistance))
                    {
                        if (hit.collider.CompareTag("AI"))
                        {
                            visibleAgents.Add(hit.transform);
                        }
                    }
                }
            }
        }

        if (visibleAgents.Count > 0)
        {
            if (isSeeker)
            {
                isChasing = false;
            }
        }
        else
        {
            if (!isSeeker)
            {
                isChasing = true;
            }
        }

    }
    #endregion

    #region Flee
    private void FleeFromSeeker()
    {
        if (currentTarget != null && seeker != null)
        {
            // Calculate the direction from the seeker to the AI.
            Vector3 fleeDirection = transform.position - seeker.position;

            // Normalize the direction and multiply by fleeDistance.
            fleeDirection = fleeDirection.normalized * fleeDistance;

            // Calculate the destination point.
            Vector3 destinationPoint = transform.position + fleeDirection;

            // Set the AI's destination to the new point.
            m_NavMeshAgent.SetDestination(destinationPoint);
            m_NavMeshAgent.speed = fleeSpeed;
        }
    }
    #endregion

    #region Chase

    private void ChaseTarget()
    {
        if (currentTarget != null)
        {
            // Calculate the path to the detected agent.
            m_NavMeshAgent.SetDestination(currentTarget.position);

            // Set the AI's speed to the chase speed.
            m_NavMeshAgent.speed = chaseSpeed;
        }
    }
    #endregion


}
