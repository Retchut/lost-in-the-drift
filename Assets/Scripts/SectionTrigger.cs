using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject[] roadSections; // Array of prefabs to choose from
    private float sectionSpacing = 40f; // Spacing between sections along the z-axis

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Trigger")) 
        {
            // Check if there are road sections to choose from
            if (roadSections.Length > 0)
            {
                // Calculate the new z position based on the number of instantiated sections
                // float newZ = transform.position.z + sectionSpacing;
                float newZ = 60;
                // Randomly select a prefab from the array
                int randomIndex = Random.Range(0, roadSections.Length);
                GameObject selectedRoadSection = roadSections[randomIndex];
                Debug.Log(selectedRoadSection + "|" + sectionSpacing + "|" + transform.position.z + "|" + newZ);

                // Instantiate the selected prefab with the new position
                Instantiate(selectedRoadSection, new Vector3(0, 0, newZ), Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("No road sections available.");
            }
        }
    }
}
