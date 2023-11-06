using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FOVVisualizer : MonoBehaviour
{
    public Transform agentTransform; // Reference to the agent's transform
    public float fieldOfView = 90f; // FOV angle in degrees
    public float viewDistance = 5f; // Max distance for FOV visualization
    public int rayCount = 60; // Number of rays to cast for FOV visualization

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = rayCount + 1;
        lineRenderer.SetPosition(0, transform.localPosition + new Vector3(0,.5f,0));
    }

    private void Update()
    {
        DrawFOV();
    }

    private void DrawFOV()
    {
        float stepAngle = fieldOfView / rayCount;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = -fieldOfView / 2 + i * stepAngle;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * agentTransform.forward;

            if (Physics.Raycast(agentTransform.position, direction, out RaycastHit hit, viewDistance))
            {
                Debug.DrawLine(agentTransform.localPosition, hit.point, Color.green);
            }
            else
            {
                Debug.DrawRay(agentTransform.localPosition, direction * viewDistance, Color.red);
            }
        }
    }
}
