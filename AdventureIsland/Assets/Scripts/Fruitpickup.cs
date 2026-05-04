using UnityEngine;

public class Fruitpickup : CollectibleBase
{
    //public float lifeTime = 4f;
    
    //private void Start()
    //{
    //    Destroy(gameObject, lifeTime);
    //}
    protected override void OnCollected(GameObject player)
    {
        //Everything handled in base class for now.
    }
}
