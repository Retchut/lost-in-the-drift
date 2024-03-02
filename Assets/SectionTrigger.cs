using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject roadSection;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Trigger")) 
        {
            Instantiate(roadSection, new Vector3(11, 2, transform.position.z + 61), Quaternion.identity);
        }
    }
}
