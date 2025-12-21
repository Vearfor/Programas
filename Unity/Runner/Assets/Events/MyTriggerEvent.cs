using UnityEngine;
using UnityEngine.Events;

public class MyTriggerEvent : MonoBehaviour
{
    [SerializeField] UnityEvent onEnter;
    [SerializeField] string tagActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == tagActivate)
        {
            onEnter.Invoke();
        }
    }
}
