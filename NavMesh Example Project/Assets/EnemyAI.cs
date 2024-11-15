using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WayPoint
{
    // Waypoint we want our NPC to move towards
    public GameObject waypoint;
}

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chase,
        LostSight
    }

    public State state;
    public GameObject target;
    public UnityEngine.AI.NavMeshAgent agent;
    public WayPoint[] patrolPoints;

    private bool isPlayerInCone = false;
    private int patternIndex = 0;
    private bool speedChanged = false;
    private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Patrol;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Patrol:
                patrolBehaviour();
                checkForPlayer();
                break;
            case State.Chase:
                chaseBehaviour();
                checkForPlayer();
                break;
            case State.LostSight:
                lostSightBehaviour();
                checkForPlayer();
                break;
        }
    }

    private void patrolBehaviour()
    {
        if (speedChanged)
        {
            agent.speed -= 1;
            speedChanged = false;
        }
        WayPoint wayPointCD = patrolPoints[patternIndex];

        //Setting a new patrolPoint when needed
        if (agent.remainingDistance <= 0.1)
        {
            agent.SetDestination(wayPointCD.waypoint.transform.position);

            if (patternIndex == patrolPoints.Length - 1)
            {
                patternIndex = 0;
            }
            else
            {
                patternIndex++;
            }
        }
    }

    private void chaseBehaviour()
    {
        if (!speedChanged)
        {
            agent.speed += 1;
            speedChanged = true;
        }
        agent.SetDestination(target.transform.position);
    }

    private void lostSightBehaviour()
    {
        if(agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete && !paused)
        {
            paused = true;
            pauseAfterReachingDestination(5f);
        }
    }

    private void checkForPlayer()
    {
        if (isPlayerInCone)
        {
            Ray ray = new Ray(gameObject.transform.position, gameObject.transform.position - target.transform.position);
            RaycastHit hitData;

            if(Physics.Raycast(ray, out hitData))
            {
                if(hitData.transform.gameObject.tag == "Player")
                {
                    state = State.Chase;
                }
                else if(state == State.Chase)
                {
                    state = State.LostSight;
                }
            }
            else if(state == State.Chase)
            {
                state = State.LostSight; 
            }
        }
        else
        {
            state = State.LostSight;
        }
            //cast a ray
            //if ray hits
                //state is changed to chase mode
            //else if it does not hit AND state is chase
                //state is changed to lost sight
        //else if player not in cone
            //state is changed to lost sight
    }

    IEnumerator pauseAfterReachingDestination(float time)
    {
        yield return new WaitForSeconds(time);
        paused = false;
        if(state == State.LostSight)
        {
            state = State.Patrol;
            agent.SetDestination(patrolPoints[patternIndex].waypoint.transform.position);
        }
    }

    public void playerEnteredCone()
    {
        isPlayerInCone = true;
    }

    public void playerExitedCone()
    {
        isPlayerInCone = false;
    }
}