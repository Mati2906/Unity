using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Characters.FirstPerson;

public class Patrol : MonoBehaviour
{

    public List<Transform> points;
    public ThirdPersonCharacter character { get; private set; }

    private bool Running = false;
    private Vector3? target;
    private int destPoint = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();

        agent.autoBraking = true;
        agent.updateRotation = false;
        agent.updatePosition = true;

        GotoNextPoint();
    }
    public void Run()
    {
        Running = true;
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Count == 0)
            return;

        // Set the agent to go to the currently selected destination.
        if(points[destPoint] != null)
            agent.SetDestination(points[destPoint].position);
        destPoint = (destPoint + 1) % points.Count;
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }

    void Update()
    {
        if (target.HasValue)
            agent.SetDestination(target.Value);

        if (agent.remainingDistance > agent.stoppingDistance)
            character.Move(agent.desiredVelocity, !Running, false);
        else
        {
            if (target.HasValue)
                target = null;
            Running = false;
            GotoNextPoint();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<FirstPersonController>().DieOnCollisionWithMonster();
            enabled = false;

        }

    }
}