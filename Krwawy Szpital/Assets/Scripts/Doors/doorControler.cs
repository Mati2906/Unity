using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class DoorControler : MonoBehaviour
{
    public float RotateInSecond;
    public int RotateAngles;
    [SerializeField] private bool isOpen;
    public bool revertRotation;
    public bool locked;
    public int keyNumber;
    public bool isDestroyed;

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
        int Angles = RotateAngles * (revertRotation ? -1 : 1) * (openClose ? 1 : -1); ;
        float speed = Time.deltaTime;
        Vector3 turnIncrement = new Vector3(0, Angles / (RotateInSecond/ speed), 0);

        for (float time = 0 ; time < RotateInSecond ; time += speed)
        {
            stateSpin.eulerAngles +=turnIncrement;
            transform.rotation = stateSpin;
            yield return null;
        }

        spinning = false;
    }
}

