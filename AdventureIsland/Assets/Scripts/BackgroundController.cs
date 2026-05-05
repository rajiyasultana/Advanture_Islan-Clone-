using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    public GameObject Camera;
    public float parallaxEffect;
    public float length;
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float distance = Camera.transform.position.x * parallaxEffect;
        float movement = Camera.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);//bg position update

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
}
