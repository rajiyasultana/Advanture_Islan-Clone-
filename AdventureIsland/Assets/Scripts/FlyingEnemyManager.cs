using UnityEngine;
using System.Collections;

public class FlyingEnemyManager : MonoBehaviour
{
    public GameObject[] flyingEnemies;
    public float activationDelay = 2f;

    private bool started = false;

    public void StartFlyingEnemies()
    {
        if (started) return;

        started = true;
        StartCoroutine(ActivateEnemiesRoutine());
    }

    private IEnumerator ActivateEnemiesRoutine()
    {
        foreach (GameObject enemy in flyingEnemies)
        {
            enemy.SetActive(true);

            yield return new WaitForSeconds(activationDelay);
        }
    }
}
