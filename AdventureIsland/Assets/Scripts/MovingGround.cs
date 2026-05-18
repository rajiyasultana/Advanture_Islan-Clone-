using UnityEngine;

public class MovingGround : MonoBehaviour
{
    
    public float floatDistance = 1.5f;
    public float floatSpeed = 2f;

    //private Rigidbody rb;
    private Vector3 startPosition;
    private Vector3 originalScale;

    void Start()
    {
        startPosition = transform.position;
        //rb = GetComponent<Rigidbody>();

        
    }

    void FixedUpdate()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatDistance;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

    }

    private void OnTriggerEnter(Collider other)
    {
        originalScale = other.transform.localScale;
        other.transform.SetParent(transform);

        other.transform.localScale = originalScale;
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }

}
