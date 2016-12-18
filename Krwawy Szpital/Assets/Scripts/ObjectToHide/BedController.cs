using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class BedController : MonoBehaviour
{

    public float moveSpeed;

    private Vector3 positionBeforeHide;
    private Vector3 positionToHide;
    private Transform playerTransform;
    private bool isHide = false;
    private bool hidingOrShowing = false;

    public bool Use()
    {
        if(!hidingOrShowing)
        {
            if (isHide)
            {
                isHide = false;
                StartCoroutine(Show());
                GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>().setBed();
            }
            else
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                positionBeforeHide = playerTransform.position;
                positionToHide = Vector3.Distance(positionBeforeHide, transform.GetChild(0).position) > Vector3.Distance(positionBeforeHide, transform.GetChild(1).position) ? transform.GetChild(0).position : transform.GetChild(1).position;
                isHide = true;
                StartCoroutine(Hide());
            }
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<FirstPersonController>().setBed(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<FirstPersonController>().setBed();
        }
    }

    IEnumerator Hide()
    {
        hidingOrShowing = true;

        float distanceY = (positionBeforeHide.y - positionToHide.y);
        distanceY = (distanceY < 0 ? -distanceY : distanceY) / moveSpeed;

        float distanceX = (positionBeforeHide.x - positionToHide.x);
        distanceX = distanceX / moveSpeed;

        float distanceZ = (positionBeforeHide.z - positionToHide.z);
        distanceZ = distanceZ / moveSpeed;

        Vector3 stateHide = playerTransform.position;
        Vector3 hideIncrement = new Vector3(0, distanceY, 0);

        for (int i = 0; i < moveSpeed; i++)
        {
            stateHide -= hideIncrement;
            playerTransform.position = stateHide;
            yield return null;
        }

        hideIncrement = new Vector3(distanceX, 0, distanceZ);
        for (int i = 0; i < moveSpeed; i++)
        {
            stateHide -= hideIncrement;
            playerTransform.position = stateHide;
            yield return null;
        }

        hidingOrShowing = false;
    }

    IEnumerator Show()
    {
        hidingOrShowing = true;

        float distanceY = (positionBeforeHide.y - positionToHide.y);
        distanceY = (distanceY < 0 ? -distanceY : distanceY) / moveSpeed;

        float distanceX = (positionBeforeHide.x - positionToHide.x);
        distanceX = distanceX / moveSpeed;

        float distanceZ = (positionBeforeHide.z - positionToHide.z);
        distanceZ = distanceZ / moveSpeed;

        Vector3 stateHide = playerTransform.position;
        Vector3 hideIncrement = new Vector3(distanceX, 0, distanceZ);

        for (int i = 0; i < moveSpeed; i++)
        {
            stateHide += hideIncrement;
            playerTransform.position = stateHide;
            yield return null;
        }

        hideIncrement = new Vector3(0, distanceY, 0);
        for (int i = 0; i < moveSpeed; i++)
        {
            stateHide += hideIncrement;
            playerTransform.position = stateHide;
            yield return null;
        }

        hidingOrShowing = false;
    }
}
