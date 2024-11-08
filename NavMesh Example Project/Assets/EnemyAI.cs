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
                break;
            case State.Chase:
                chaseBehaviour();
                break;
            case State.LostSight:
                lostSightBehaviour();
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
}