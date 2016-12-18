using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class DoorControler : MonoBehaviour
{
    public float turnSpeed;
    [SerializeField] private bool isOpen;
    public bool revertRotation;
    public bool locked;
    public int keyNumber;
    public bool isDestroyed;

    private Vector3 startRotation;
    private Vector3 endRotation;

    //are we currently spinning?
    //we use this to prevent multiple instance of spinning starting.
    private bool spinning = false;

    public void Use(bool key)
    {
        if (locked && key)
            locked = false;

        if(!spinning && !isDestroyed && !locked)
            if (isOpen)
                Close();
            else
                Open();
    }

    public void Destroy()
    {
        isDestroyed = true;
        StopAllCoroutines();
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = new RigidbodyConstraints();
    }

    private void Start()
    {
        startRotation = transform.rotation.eulerAngles;
        endRotation = transform.rotation.eulerAngles + new Vector3(0, (revertRotation ? -90 : 90), 0);
    }

    private void Open()
    {
        StartCoroutine(Spin(true));
        isOpen = true;
    }

    private void Close()
    {
        StartCoroutine(Spin(false));
        isOpen = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<FirstPersonController>().setDoor(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<FirstPersonController>().setDoor();
        }
    }



    //Open  - true
    //Close - false
    IEnumerator Spin(bool openClose)
    {
        spinning = true;
        var stateSpin = transform.rotation;
        Vector3 turnIncrement = new Vector3(0, turnSpeed * Time.deltaTime, 0);

        if (openClose)
            while ((!revertRotation && transform.rotation.eulerAngles.y < endRotation.y) || 
                (revertRotation && transform.rotation.eulerAngles.y > endRotation.y))
            {
                stateSpin.eulerAngles += revertRotation ? -turnIncrement : turnIncrement;
                transform.rotation = stateSpin;
                yield return null;
            }
        else
            while ((!revertRotation && transform.rotation.eulerAngles.y > startRotation.y) || 
                (revertRotation && transform.rotation.eulerAngles.y < startRotation.y))
            {
                stateSpin.eulerAngles -= revertRotation ? -turnIncrement : turnIncrement;
                transform.rotation = stateSpin;
                yield return null;
            }
        spinning = false;
    }


}
