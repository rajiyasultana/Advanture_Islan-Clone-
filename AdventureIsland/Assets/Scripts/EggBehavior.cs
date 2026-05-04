using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EggBehavior : CollectibleBase
{
    [Header("Egg Settings")]
    [Tooltip("Assign the specific pickup prefab to spawn (e.g., Axe, Skateboard, or Angel)")]
    public GameObject itemToSpawn;
    public float throwForceX = 4f;
    public float throwForceY = 6f;
    public float timeUntilHatch = 0.8f;

    private Rigidbody rb;
    private bool isHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void Collect(GameObject player)
    {
        if (isHit) return;
        isHit = true;

        if (scoreValue > 0)
        {
            ScoreSystem playerScore = FindObjectOfType<ScoreSystem>();
            if (playerScore != null)
            {
                playerScore.AddScore(scoreValue);
            }
        }

        rb.linearVelocity = new Vector3(throwForceX, throwForceY, rb.linearVelocity.z);

        StartCoroutine(BreakEgg(player));
    }

    protected override void OnCollected(GameObject player) { }

    private IEnumerator BreakEgg(GameObject player)
    {
        yield return new WaitForSeconds(timeUntilHatch);

        if (itemToSpawn != null)
        {
            Instantiate(itemToSpawn, transform.position, Quaternion.identity);
        }

        DestroyCollectible();
    }
}
