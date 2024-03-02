using UnityEngine;

public class RoadController : MonoBehaviour
{
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
            Destroy(gameObject);
        }
    }
}
