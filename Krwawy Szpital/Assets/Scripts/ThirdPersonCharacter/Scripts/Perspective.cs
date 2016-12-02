using UnityEngine;
using System.Collections;

public class Perspective : Sense
{
    public int fieldOfView;
    public int ViewDistance;

    private SphereCollider col;
    private Transform player;
    private Patrol patrol;

    protected override void Initialize()
    {
        player = GameObject.Find("Player").transform;
        patrol = GetComponent<Patrol>();
        col = GetComponent<SphereCollider>();
    }

    protected override void UpdateSense()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= detectionRate)
        {
            DetectAspect();
        }
        OnDrawGizmos();
    }

    void OnTriggerStay(Collider other)
    {
        Aspect aspect = other.GetComponent<Aspect>();

        if (aspect != null && aspect.aspectName == aspectName)
        {
            Debug.Log("Mięsko do mnie przyszło!!!");
            patrol.SetTarget(other.GetComponent<Transform>().position);
        }
    }

    void DetectAspect()
    {
        RaycastHit hit;
        Vector3 direction;

        direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle < fieldOfView && Physics.Raycast(transform.position, direction, out hit, ViewDistance))
        {
            Aspect aspect = hit.collider.GetComponent<Aspect>();
            if (aspect != null && aspect.aspectName == aspectName)
            {
                Debug.Log("Widze cię moje mięsko!!!");
                patrol.Run(hit.collider.GetComponent<Transform>().position);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (player == null)
            return;

        Debug.DrawLine(transform.position, player.position, Color.red);

        Vector3 frontRayPoint = transform.position + (transform.forward * ViewDistance);

        Vector3 leftRayPoint = frontRayPoint;
        leftRayPoint.x += fieldOfView * 0.5f;

        Vector3 rightRayPoint = frontRayPoint;
        rightRayPoint.x -= fieldOfView * 0.5f;

        Debug.DrawLine(transform.position, frontRayPoint, Color.green);
        Debug.DrawLine(transform.position, leftRayPoint, Color.green);
        Debug.DrawLine(transform.position, rightRayPoint, Color.green);
    }
}
