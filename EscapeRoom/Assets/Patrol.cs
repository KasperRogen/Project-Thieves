using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class Patrol : MonoBehaviour
{
    [SerializeField]
    List<Vector3> PatrolPoints;
    [SerializeField]
    [Range(0, 10)]
    float IdleTime = 5;

    int pointIndex;
    NavMeshAgent agent;
    float doneTime;

    [HideInInspector]
    public GameObject player;


    Animator anim;

    [Header("Vision")]
    [SerializeField]
    float visionDistance;
    [SerializeField]
    float visionAngle;


    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        doneTime = Time.time;
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(PatrolPoints[pointIndex = UnityEngine.Random.Range(0, PatrolPoints.Count - 1)]);
    }

    // Update is called once per frame
    void Update()
    {

        
    }
    
    private void FixedUpdate()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);
        anim.SetFloat("AnimSpeed", agent.velocity.magnitude / agent.speed);
    }

    [Task]
    public void IdlePatrol()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }


        else if(Vector3.Distance(transform.position, agent.destination) < 2)
        {
            agent.speed = 2;
            if(Time.time - doneTime > IdleTime)
            {
                agent.SetDestination(PatrolPoints[pointIndex = pointIndex + 1 != PatrolPoints.Count ? ++pointIndex : 0]);
                doneTime = Time.time;
            }
        }

        Task.current.Succeed();
    }
    
    
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;


        Gizmos.color = Color.yellow;
        for(int i = 0; i < PatrolPoints.Count; i++)
        {
            Gizmos.DrawCube(PatrolPoints[i], new Vector3(1, 1.8f, 1));


            Gizmos.DrawLine(PatrolPoints[i], i + 1 != PatrolPoints.Count ? PatrolPoints[i + 1] : PatrolPoints[0]);
        }
    }

}
