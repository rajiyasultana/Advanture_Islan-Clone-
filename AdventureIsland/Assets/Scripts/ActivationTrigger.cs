using UnityEngine;
using System.Collections;

public class ActivationTrigger : MonoBehaviour
{
    [Header("Stone Activation Settings")]
    [Tooltip("Drag the inactive stone GameObjects here.")]
    public GameObject[] stonesToActivate;

    [Tooltip("Time in seconds between each stone appearing.")]
    public float delayBetweenStones = 1.0f;

    private bool hasTriggered = false;

    private void Awake()
    {
        // Ensure the collider is set to act as a trigger
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Prevent it from triggering multiple times
        if (hasTriggered) return;

        // Check if the object entering the trigger is the Player
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(ActivateStonesRoutine());
        }
    }

    private IEnumerator ActivateStonesRoutine()
    {
        // Loop through the array of stones one by one
        foreach (GameObject stone in stonesToActivate)
        {
            if (stone != null)
            {
                stone.SetActive(true);
                yield return new WaitForSeconds(delayBetweenStones); // Wait before activating the next one
            }
        }
    }
}
