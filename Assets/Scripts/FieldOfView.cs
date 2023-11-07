using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    private float fov;
    private float viewDistance;
    private Vector3 origin;
    private float startingAngle;
    public  Collider hitCollider;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        UpdateFOVMesh();
    }

    private void LateUpdate()
    {
        UpdateFOVMesh();
    }

    void UpdateFOVMesh()
    {
        int numRays = 30; // Adjust as needed for smoother FOV representation
        float halfFOV = fov * 0.5f;

        Vector3[] vertices = new Vector3[numRays + 1];
        int[] triangles = new int[numRays * 3];

        vertices[0] = Vector3.zero;

        float angleIncrement = fov / numRays;

        for (int i = 0; i < numRays; i++)
        {
            float angle = -halfFOV + angleIncrement * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, direction, out hit, viewDistance, layerMask))
            {
                vertices[i + 1] = transform.InverseTransformPoint(hit.point);
            }
            else if (Physics.Raycast(transform.position, direction, out hit, viewDistance))
            {
                if (hit.collider.TryGetComponent<HiderAgent>(out HiderAgent agent))
                {
                    hitCollider = hit.collider;
                }
            }
            else
            {
                vertices[i + 1] = transform.InverseTransformPoint(transform.position + direction * viewDistance);
            }

            if (i < numRays - 1)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public Collider GetHitInfo()
    {
        if (hitCollider != null)
        {
            return hitCollider;
        }
        return null;
    }


    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }

    public void SetFoV(float fov)
    {
        this.fov = fov;
    }

    public void SetViewDistance(float viewDistance)
    {
        this.viewDistance = viewDistance;
    }

    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
}
