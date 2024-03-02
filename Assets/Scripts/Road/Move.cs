using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (transform != null)
        {            
            transform.position += Vector3.back * Time.deltaTime;
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
