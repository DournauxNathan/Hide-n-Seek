using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] private bool canHide;
    [SerializeField] private bool isTaken;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Hider"))
        {
            canHide = false;
            isTaken = true;
        }
        else
        {
            canHide = true;
        }
    }

    public void Taken()
    {
        canHide = false;
        isTaken = true;
    }

    public bool CanHide()
    {
        return canHide;
    }

    public void Reset()
    {
        canHide = true;
        isTaken = false;
    }
}
