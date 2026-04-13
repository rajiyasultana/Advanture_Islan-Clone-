using System;
using UnityEngine;
using UnityEngine.Events;

public class UniversalTrigger : MonoBehaviour
{
    [SerializeField] bool distroyOnTriggerEnter;
    [SerializeField] string tagFilter;
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;

    void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) {
            return;
        }
        onTriggerEnter.Invoke();
        if (distroyOnTriggerEnter) {
            Destroy(gameObject);
        }
    }

    void OnTriggerExit(Collider other) {
        if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter))
        {
            return;
        }
        onTriggerExit.Invoke();
    }
}
