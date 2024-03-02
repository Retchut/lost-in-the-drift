using UnityEngine;

public class Move : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 4f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is used for physics updates
    void Update()
    {
        float distance = speed * Time.fixedDeltaTime;
        // Move the Rigidbody along the z-axis
        rb.MovePosition(rb.position + Vector3.back * distance);
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
