using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    private List<Transform> rings = new List<Transform>();

    public Material activeRing;
    public Material inactiveRing;
    public Material finalRing;

    private int ringPassed = 0;

    private void Start()
    {
        // Set the objective field in the game scene 
        FindObjectOfType<GameScene>().objective = this;

        // all rings are inactive 
        foreach (Transform t in transform)
        {
            rings.Add(t);
            t.GetComponent<MeshRenderer>().material = inactiveRing;
        }

        // do we have rings?
        if (rings.Count == 0)
        {
            Debug.Log("No rings on this level");
            return;
        }

        // activate first ring 
        rings[ringPassed].GetComponent<MeshRenderer>().material = activeRing;
        rings[ringPassed].GetComponent<Ring>().ActiveRing();
    }

    public void NextRing()
    {
        // play FX current ring 
        rings[ringPassed].GetComponent<Animator>().SetTrigger("collectionTrigger");

        // up score value 
        ringPassed++;

        // if final ring call victory 
        if (ringPassed == rings.Count)
        {
            Victory();
            return;
        }

        // if second to last give next ring final material 
        if (ringPassed == rings.Count - 1)
        {
            rings[ringPassed].GetComponent<MeshRenderer>().material = finalRing;
        }
        else
        {
            rings[ringPassed].GetComponent<MeshRenderer>().material = activeRing;
        }

        // Need to activate the ring 
        rings[ringPassed].GetComponent<Ring>().ActiveRing();
    }

    public Transform GetCurrentRing()
    {
        return rings[ringPassed];
    }

    private void Victory()
    {
        FindObjectOfType<GameScene>().CompleteLevel();
    }
}

