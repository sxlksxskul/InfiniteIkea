using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    public Camera cam;
    public NavMeshAgent agent;
    public GameObject target;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            agent.SetDestination(target.transform.position);
        }
    }
}