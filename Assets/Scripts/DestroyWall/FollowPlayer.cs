using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    [SerializeField]
    private float offset = -150f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (!player)
        {
            Debug.LogError("Unable to get player transform");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + offset);
    }
}
