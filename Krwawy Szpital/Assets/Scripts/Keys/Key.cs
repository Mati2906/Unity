using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class Key : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<FirstPersonController>().setKeyIndex(int.Parse(name.Remove(0, 3)));
            Destroy(gameObject);
        }
    }


}
