using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMeshAreaModification : MonoBehaviour
{
    // Example method to modify the area cost dynamically.
    public void ModifyAreaCost(int areaType, float newAreaCost)
    {
        NavMesh.SetAreaCost(areaType, newAreaCost);
    }
}