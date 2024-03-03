using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCarSpawner : MonoBehaviour
{
    Transform player;

    [SerializeField]
    public float timer = 3.0f;
    [SerializeField]
    public float spawnChance = 0.5f;

    [SerializeField]
    GameObject[] npcarVariants;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (!player)
        {
            Debug.LogError("Unable to retrieve Player transform");
        }
        InvokeRepeating("SpawnCar", 0.0f, timer);

    }

    void SpawnCar()
    {
        if (Random.value < spawnChance && player && npcarVariants.Length > 0)
        {
            // Randomly select a prefab from the array
            int randomIndex = Random.Range(0, npcarVariants.Length);
            GameObject variant = npcarVariants[randomIndex];

            // Instantiate the selected prefab with the new position
            Instantiate(variant, new Vector3(0, 0.5f, player.position.z + 150.0f), Quaternion.Euler(0, 180, 0));
        }
    }
}
