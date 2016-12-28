using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Characters.FirstPerson;

public class Patrol : MonoBehaviour
{
    public List<Transform> points;
    public MonsterController monster { get; private set; }
    private bool Running = false;
    private Vector3? target;
    private int destPoint = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        monster = GetComponent<MonsterController>();

        agent.autoBraking = true;
        agent.updateRotation = false;
        agent.updatePosition = true;

        GotoNextPoint();
    }

    void Update()
    {
        if (target.HasValue)
            agent.SetDestination(target.Value);

        if (agent.remainingDistance > agent.stoppingDistance)
            monster.Move(agent.desiredVelocity, Running ? 4f : 1f);
        else
        {
            if (target.HasValue)
                target = null;
            Running = false;
            GotoNextPoint();
        }
    }

    public void Run(Vector3 to)
    {
        target = to;
        Running = true;
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }

    void GotoNextPoint()
    {
        if (points.Count == 0)
        {
            monster.Move(agent.desiredVelocity, 0);
            return;
        }
            

        if(points[destPoint] != null)
            agent.SetDestination(points[destPoint].position);
        destPoint = (destPoint + 1) % points.Count;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<FirstPersonController>().DieOnCollisionWithMonster();
        }
        if (collision.gameObject.tag == "Door")
        {
            collision.gameObject.GetComponent<DoorControler>().Destroy();
        }
    }
}