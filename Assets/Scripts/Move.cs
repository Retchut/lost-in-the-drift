using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 4f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (transform != null)
        {            
            float distance = speed * Time.deltaTime;
            // Move the GameObject along the z-axis
            transform.position += Vector3.back * distance;
        }
        else
        {
            // Handle the case where the transform is null, for example:
            Debug.LogWarning("Transform is null!");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy"))
        {
            // Destroy the GameObject this script is attached to
            Debug.Log("I'm being destroyed");
            Destroy(gameObject);
        }
    }
}
