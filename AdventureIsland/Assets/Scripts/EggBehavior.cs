using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EggBehavior : CollectibleBase
{
    [Header("Egg Settings")]
    public GameObject axePrefab;       
    public float throwForceX = 4f;     
    public float throwForceY = 6f;     
    public float timeUntilHatch = 0.8f; 

    private Rigidbody rb;
    private bool isHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // We override Collect to stop standard immediate destruction
    protected override void Collect(GameObject player)
    {
        if (isHit) return;
        isHit = true;
        
        // Base add score
        if (scoreValue > 0)
        {
            ScoreSystem playerScore = FindObjectOfType<ScoreSystem>();
            if (playerScore != null)
            {
                playerScore.AddScore(scoreValue);
            }
        }

        // Pop up the egg before it hatches
        rb.linearVelocity = new Vector3(throwForceX, throwForceY, rb.linearVelocity.z);
        
        // Start the hatching process
        StartCoroutine(BreakEgg(player));
    }

    // We don't use OnCollected directly since we use a Coroutine for the delay
    protected override void OnCollected(GameObject player) { }

    private IEnumerator BreakEgg(GameObject player)
    {
        yield return new WaitForSeconds(timeUntilHatch);

        // Spawn the axe
        if (axePrefab != null)
        {
            Instantiate(axePrefab, transform.position, Quaternion.identity);
        }

        DestroyCollectible();
    }
}
