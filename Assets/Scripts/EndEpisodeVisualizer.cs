using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEpisodeVisualizer : MonoBehaviour
{
    public Phase currentPhase = Phase.Hide;

    [SerializeField] private MeshRenderer floorMeshRenderer; // Visualizer for the floor
    [SerializeField] private Material winMaterial; // Material for winning state
    [SerializeField] Material loseMaterial; // Material for losing state

    public enum Phase
    {
        Hide,
        Seek
    }

    public void ResetPhase()
    {
        currentPhase = Phase.Hide;
    }

    public void SwitchPhase()
    {
        currentPhase = Phase.Seek;
    }

    public void HiderFound()
    {
        floorMeshRenderer.material = loseMaterial;
    }

    public void HiderNotFound()
    {
        floorMeshRenderer.material = winMaterial;

    }
}
