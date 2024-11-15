using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCone : MonoBehaviour
{
    private EnemyAI parentScript;

    private void Start()
    {
        parentScript = gameObject.transform.parent.gameObject.GetComponent<EnemyAI>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            parentScript.playerEnteredCone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            parentScript.playerExitedCone();
        }
    }
}