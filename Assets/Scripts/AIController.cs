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
        Handles.color = new Color(1f, 0f, 0f, .5f); // Red with transparency
        Handles.DrawSolidArc(fromPosition, Vector3.up, viewAngleA, fov.viewAngle, fov.viewDistance);

        // Optionally, you can also draw a line to the endpoint for reference
        Handles.DrawLine(fromPosition, toPositionA);


        Handles.color = Color.yellow;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.explorationRadius);
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

    [Header("MOVEMENT")]
    public float speed = 3.5f;
    [Tooltip("Maximum distance for exploration.")] public float explorationRadius = 20.0f;

    private float timeLeftToLook;
    private float nextMoveTime;
    private bool isMovingAround = true;
    private bool isMovingToExplorationPoint = false;
    private List<Vector3> pointAlreadyVisited = new List<Vector3>();    

    [Header("REFERENCES")]
    [Tooltip("The reference of the currently detected agent to chase")] [SerializeField] private Transform currentTarget;
    [Tooltip("The reference of all the hiding spots found by an agent")] [SerializeField] private List<GameObject> hidingSpots; 

    [Header("Field Of View Parameters")]
    public Transform eyes;
    public LayerMask layerMask;
    [Space(5)]
    [Range(0f, 360f)] public float viewAngle;
    public float viewDistance;

    private NavMeshAgent m_NavMeshAgent;
    private LineRenderer m_LineRenderer; // The LineRenderer when seeker detect a hider.

    // Start is called before the first frame update
    private void Start()
    {
        

        if (role == Role.Seeker)
        {
            GameManager.instance.SubscribeSeekers(this);
        }
        else
        {
            GameManager.instance.SubscribeHiders(this);
        }

    
        m_LineRenderer = GetComponent<LineRenderer>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
     
        m_LineRenderer.enabled = false; // Initially hide the LineRenderer.

        timeLeftToLook = GameManager.instance.hidingTime;
    }

    // Update is called once per frame
    private void Update()
    {
        SetSpeed();

        if (role == Role.Seeker && GameManager.instance.isGameOn)
        {
            LookAround();
            DetectHider();
        }

        if (role == Role.Hider)
        {
            timeLeftToLook -= Time.deltaTime;

            DetectHidingSpots();

            // If the timer is near the end, call MoveToFarthestHidingSpot.
            if (timeLeftToLook <= GameManager.instance.hidingTime / 3)
            {
                isMovingAround = false;
                isMovingToExplorationPoint = false;
                timeLeftToLook = GameManager.instance.hidingTime;

                MoveToFarthestHidingSpot();
            }
        }
    }

    #region Move

    public void SetSpeed()
    {
        m_NavMeshAgent.speed = speed;
    }

    public void LookAround()
    {
        if (isMovingAround)
        {
            // Check if it's time to stop moving around and start looking for hiding spots.
            if (Time.time >= nextMoveTime)
            {
                isMovingAround = false;

                //If there is no point already visited, Add the current position in the list
                if (pointAlreadyVisited.Count == 0)
                {
                    // Initial exploration - add the current position.
                    pointAlreadyVisited.Add(transform.position);
                }

                int maxAttempts = 20; // Adjust this value as needed to avoid getting stuck.

                for (int i = 0; i < maxAttempts; i++)
                {
                    // Choose a random exploration point that hasn't been explored yet.
                    Vector3 randomExplorationPoint = GetRandomExplorationPoint();

                    // Check if the position is not in the list of explored positions.
                    if (!pointAlreadyVisited.Contains(randomExplorationPoint))
                    {
                        // Explore the new position.
                        pointAlreadyVisited.Add(randomExplorationPoint);

                        isMovingToExplorationPoint = true;
                        // Set the AI's destination to the exploration point.
                        MoveTo(randomExplorationPoint);

                        return; // Break out of the loop once a valid unexplored position is found.
                    }
                }

            }
        }
        else
        {     
            // If the AI has reached the exploration point, stop moving.
            if (isMovingToExplorationPoint && m_NavMeshAgent.remainingDistance < 5f)
            {
                isMovingToExplorationPoint = false;
                isMovingAround = true; // Resume moving around after finding spots.

            }
        }
    }

    public void MoveTo(Vector3 position)
    {
        m_NavMeshAgent.SetDestination(position);

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

    private void DetectHidingSpots()
    {
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
                    Transform hidingSpot = target.transform;

                    hidingSpots.Add(hidingSpot.gameObject);
                }
            }
        }
    }

    private void DetectHider()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewDistance, layerMask);
        foreach (Collider target in targetsInViewRadius)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;


            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float angle = Vector3.Angle(directionToTarget, transform.forward);

                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToTarget, out hit, viewDistance))
                {
                    if (hit.collider.CompareTag("Hider"))
                    {
                        currentTarget = target.transform;

                        // Hider is detected. Implement the action you want the Seeker to take when a Hider is found.
                        // Draw a line using the LineRenderer from the AI's position to the detected agent.// Draw a line using the LineRenderer from the AI's position to the detected agent.
                        m_LineRenderer.enabled = true;
                        m_LineRenderer.SetPosition(0, transform.position);
                        m_LineRenderer.SetPosition(1, currentTarget.position + new Vector3(0, 1.5f, 0)); // You can update this for multiple detected agents.

                        // Stop seeking and start chasing the Hider.
                        Debug.Log("End");
                        Debug.Break();
                    }
                }
                else
                {
                    m_LineRenderer.enabled = false;
                }
            }
            else
            {
                m_LineRenderer.enabled = false;
            }
        }
    }
    #endregion

    #region Hide Behaviour

    private void MoveToFarthestHidingSpot()
    {
        if (hidingSpots.Count > 0)
        {
            // Find the farthest hiding spot from the Hider's current position.
            Vector3 farthestSpot = Vector3.zero;
            float farthestDistance = 0f;

            foreach (GameObject spot in hidingSpots)
            {
                foreach (AIController seeker in GameManager.instance.seekers)
                {
                    float distance = Vector3.Distance(seeker.transform.position, spot.transform.position);

                    if (distance > farthestDistance)
                    {
                        farthestDistance = distance;
                        farthestSpot = spot.transform.position;
                    }
                }
            }

            if (farthestSpot != Vector3.zero)
            {
                // Go to the farthest hiding spot.
                MoveTo(farthestSpot);
                hidingSpots.Clear();
            }
        }
    }
    #endregion

    private Vector3 GetRandomExplorationPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * explorationRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, explorationRadius, 1);

        return hit.position;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
