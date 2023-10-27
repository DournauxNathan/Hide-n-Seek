using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    private bool canHide;
    private bool isTaken;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Hider"))
        {
            canHide = true;
        }
        else
        {
            canHide = false;
        }
    }

    public void Taken()
    {
        isTaken = true;
    }

    public bool CanHide()
    {

        return canHide;
    }
}
