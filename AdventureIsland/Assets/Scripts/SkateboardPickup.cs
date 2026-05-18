using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SkateboardPickup : MonoBehaviour
{
    public bool isCollected = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

            if (player != null)
            {
                isCollected = true;
                player.EnableSkateboard(); // Increase player speed
                FindObjectOfType<FlyingEnemyManager>().StartFlyingEnemies();
                //SoundManager.PlaySound(SoundManager.SoundType.eggItemCollect);

                gameObject.SetActive(false);
            }
        }
    }
}
