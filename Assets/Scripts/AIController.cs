using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

[CustomEditor(typeof(AIController))]
public class FOVEditor : Editor
{
    void OnSceneGUI()
    {
        AIController fov = (AIController)target;

        // Store the AI's position
        Vector3 fromPosition = fov.eyes.position;

        // Calculate the endpoint of the FOV cone
        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 toPositionA = fromPosition + viewAngleA * fov.viewDistance;

        // Draw the filled cone to represent FOV
        Handles.color = new Color(1f, 0f, 0f, 0.2f); // Red with transparency
        Handles.DrawSolidArc(fromPosition, Vector3.up, viewAngleA, fov.viewAngle, fov.viewDistance);

        // Optionally, you can also draw a line to the endpoint for reference
        Handles.DrawLine(fromPosition, toPositionA);
    }
}

public enum Role
{
    Hider,
    Seeker
}

public class AIController : MonoBehaviour
{    
    public Role role;

    public List<GameObject> hidingSpots;

    [Space(10)]
    [Header("SEEKER")] 
    private LineRenderer m_LineRenderer; // The LineRenderer for FOV detection.
    public float chaseSpeed = 5f; // Speed at which the AI chases other agents.
    private Transform currentTarget; // The currently detected agent to chase.
    [SerializeField] private List<Transform> visibleAgents = new List<Transform>();
    
    [Space(10)]
    [Header("HIDER")]
    public Transform seeker; // Reference to


    [Space(10)]
    [Header("Field Of View Parameters")]
    public Transform eyes;
    public LayerMask layerMask;
    [Space(5)]
    [Range(0f, 360f)] public float viewAngle;
    public float viewDistance;

    private NavMeshAgent m_NavMeshAgent;

    // Start is called before the first frame update
    private void Start()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
     
        m_LineRenderer.enabled = false; // Initially hide the LineRenderer.


        timeLeftToLook = GameManager.instance.hidingTime;
    }

    // Update is called once per frame
    private void Update()
    {
        /*if (role == Role.Seeker && visibleAgents.Count != 0)
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
        }*/

    }

    #region Move

    private float explorationRadius = 10.0f; // Maximum distance for exploration.
    
    public void MoveTo(Vector3 position)
    {
        m_NavMeshAgent.SetDestination(position);
    }

    private Vector3 GetRandomExplorationPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * explorationRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, explorationRadius, 1);
        return hit.position;
    }
    #endregion

    //To DO
    #region Interaction W/ Objects
    private GameObject FindNearestInteractableObject()
    {
        return null;
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

        Collider[] colliders = Physics.OverlapSphere(transform.position, viewDistance);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("AI")) // Assuming AI agents have the "AI" tag.
            {
                Vector3 direction = collider.transform.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);

                if (angle <= viewAngle / 2)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, direction, out hit, viewDistance))
                    {
                        if (hit.collider.CompareTag("AI"))
                        {
                            visibleAgents.Add(hit.transform);
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region Hide Behaviour

    private float nextMoveTime;
    private int foundHidingSpots = 0;
    private float timeLeftToLook;
    private bool isMovingAround = true;
    private Vector3 randomExplorationPoint;
    private bool isMovingToExplorationPoint = false;
    private List<Vector3> pointAlreadyVisited = new List<Vector3>();
    
    public void LookForHidingSpots()
    {
        timeLeftToLook -= Time.deltaTime;

        if (isMovingAround)
        {
            // Check if it's time to stop moving around and start looking for hiding spots.
            if (Time.time >= nextMoveTime)
            {
                isMovingAround = false;

                // Generate a random exploration point within the specified radius.
                randomExplorationPoint = GetRandomExplorationPoint();

                // Set the AI's destination to the exploration point.
                if (!pointAlreadyVisited.Contains(randomExplorationPoint))
                {
                    isMovingToExplorationPoint = true;
                    MoveTo(randomExplorationPoint);
                }
                pointAlreadyVisited.Add(randomExplorationPoint);
            }
        }
        else
        {
            // If the AI has reached the exploration point, stop moving.
            if (isMovingToExplorationPoint && m_NavMeshAgent.remainingDistance < 0.1f)
            {
                isMovingToExplorationPoint = false;
                isMovingAround = true; // Resume moving around after finding spots.
            }

            #region Implement logic to find and add hiding spots during the lookForHidingSpotsDuration.

            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewDistance, layerMask);
            foreach (Collider target in targetsInViewRadius)
            {
                Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
                {
                    // The target is within the AI's field of view
                    float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                    // You can add more conditions here to filter the targets, e.g., check if the target is visible, within a certain distance, etc.

                    // React to the target
                    // For example, you might want to follow or attack the target.

                    //If the Target is a Hiding Spot && if is position in not in the hiding list
                    if (target.CompareTag("HidingSpot") && !hidingSpots.Contains(target.gameObject))
                    {
                        Debug.Log(target.name);

                        // You've found a hiding spot. Add it to your list of hiding spots.
                        foundHidingSpots++; 
                        Transform hidingSpot = target.transform;
                        hidingSpots.Add(hidingSpot.gameObject);
                    }
                }
            }
            #endregion
        }

        // If the timer is near the end, call MoveToFarthestHidingSpot.
        if (timeLeftToLook <= GameManager.instance.hidingTime / 3)
        {
            isMovingAround = false;
            isMovingToExplorationPoint = false;
            timeLeftToLook = GameManager.instance.hidingTime;

            MoveToFarthestHidingSpot();
        }
    }

    private void MoveToFarthestHidingSpot()
    {
        if (hidingSpots.Count > 0)
        {
            // Find the farthest hiding spot from the Hider's current position.
            Vector3 farthestSpot = Vector3.zero;
            float farthestDistance = 0f;

            foreach (GameObject spot in hidingSpots)
            {
                float distance = Vector3.Distance(seeker.position, spot.transform.position);

                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    farthestSpot = spot.transform.position;
                }
            }

            if (farthestSpot != Vector3.zero)
            {
                Debug.Log("Hiding spot reached !");

                // Go to the farthest hiding spot.
                MoveTo(farthestSpot);
            }
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
  
    public Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
