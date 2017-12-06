using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    private Objective objectiveScript;
    private bool ringActive = false;

    private void Start()
    {
        objectiveScript = FindObjectOfType<Objective>();
    }

    public void ActiveRing()
    {
        ringActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // if the ring is active 
        // tell the objective script is has been passed through

        if (ringActive)
        {
            objectiveScript.NextRing();
            Destroy(gameObject, 5.0f);
        }
    }
}