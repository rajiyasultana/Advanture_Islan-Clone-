using UnityEngine;

public class Fruitpickup : CollectibleBase
{
    public float lifeTime = 4f;

    private float elapsedTime = 0f;
    private bool isTimerActive = false;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);

        bool inView = viewPos.x > 0 && viewPos.x < 1 &&
                      viewPos.y > 0 && viewPos.y < 1 &&
                      viewPos.z > 0;

        // Start timer ONLY when entering view
        if (inView && !isTimerActive)
        {
            isTimerActive = true;
            elapsedTime = 0f;
        }

        if (!isTimerActive) return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifeTime)
        {
            gameObject.SetActive(false);
            isTimerActive = false;
        }
    }

    protected override void OnCollected(GameObject player)
    {
    }
}