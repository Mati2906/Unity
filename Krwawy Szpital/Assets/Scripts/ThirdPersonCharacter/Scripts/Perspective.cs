using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class Perspective : Sense
{
    public int fieldOfView;
    public int ViewDistance;
    public string asp;

    private Transform player;
    private FirstPersonController playerController;
    private Patrol patrol;

    protected override void Initialize()
    {
        player = GameObject.Find("Player").transform;
        playerController = GameObject.Find("Player").GetComponent<FirstPersonController>();
        patrol = GetComponent<Patrol>();
    }

    protected override void UpdateSense()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= detectionRate)
        {
            DetectAspect();
            ListeningPlayer();
        }
        OnDrawGizmos();
    }

    void OnTriggerStay(Collider other)
    {
        Aspect aspect = other.GetComponent<Aspect>();

        if (aspect != null && aspect.aspectName == aspectName)
            patrol.Run(other.GetComponent<Transform>().position);
    }
    void ListeningPlayer()
    {
        if (Vector3.Distance(player.position, transform.position) < playerController.audibility)
        {
            Debug.Log("słysze cie! - " + Vector3.Distance(player.position, transform.position));
            patrol.SetTarget(player.position);
        }
    }
    void DetectAspect()
    {
        RaycastHit hit;
        Vector3 direction;

        //czerwona linia - w stronę gracza
        direction = player.position - transform.position;

        //kąt między kierunkiem do gracza a kierunkiem patrzenia potwora
        //kąt między czerwoną linią a środkową zieloną
        float angle = Vector3.Angle(direction, transform.forward);

        //jeżeli kąt jest mniejszy od zadeklarowanego kąta widzenia ozanacza to że gracz może być w polu widzenia
        //Sprawdzamy czy obiekt jest w zasięgu pola widzenia
        if (angle < fieldOfView && Physics.Raycast(transform.position, direction, out hit, ViewDistance))
        {
            Debug.DrawLine(transform.position, player.position, Color.red);
            //sprawdzamy czy tym obiektem jest gracz
            Aspect aspect = hit.collider.GetComponent<Aspect>();
            asp = hit.transform.tag;
            if (aspect != null && aspect.aspectName == aspectName)
                patrol.Run(hit.collider.GetComponent<Transform>().position);
        }
    }

    void OnDrawGizmos()
    {
        if (player == null)
            return;

        //Debug.DrawLine(transform.position, player.position, Color.red);
        Vector3 frontRayPoint = transform.position + (transform.forward * ViewDistance);

        //Vector3 leftRayPoint = Quaternion.AngleAxis(-fieldOfView / 2, Vector3.up) * frontRayPoint;
        //Vector3 rightRayPoint = Quaternion.AngleAxis(fieldOfView / 2, Vector3.up) * frontRayPoint;

        Debug.DrawLine(transform.position, frontRayPoint, Color.green);
        //Debug.DrawLine(transform.position, leftRayPoint, Color.green);
        //Debug.DrawLine(transform.position, rightRayPoint, Color.green);
    }
}
