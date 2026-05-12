using UnityEngine;

public class DestroybehinfCamera : MonoBehaviour
{
    public float destroyDistance = 10f;

    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        if(transform.position.x < playerCamera.position.x - destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
