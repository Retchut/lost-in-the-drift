using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform tilesParent;
    public GameObject[] roadSections; // Array of prefabs to choose from
    [SerializeField]
    private float roadLength = 100f; // Length (height) of the road tiles
    [SerializeField]
    private int lookahead = 2; // how many tiles in front to instantiate (2 by default because we set up the scene with 2 road tiles)

    void Start()
    {
        tilesParent = GameObject.FindGameObjectWithTag("TilesParent").transform;
        if (!tilesParent)
        {
            Debug.LogError("Unable to get tiles parent object");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            // Check if there are road sections to choose from
            if (roadSections.Length > 0)
            {
                Transform roadTransform = other.transform.parent.transform;
                // Calculate the new z position based on the number of instantiated sections
                float newZ = roadTransform.position.z + roadLength * lookahead;

                // Randomly select a prefab from the array
                int randomIndex = Random.Range(0, roadSections.Length);
                GameObject selectedRoadSection = roadSections[randomIndex];

                // Instantiate the selected prefab with the new position
                Instantiate(selectedRoadSection, new Vector3(0, 0, newZ), Quaternion.identity, tilesParent);
            }
            else
            {
                Debug.LogWarning("No road sections available.");
            }
        }
    }
}
