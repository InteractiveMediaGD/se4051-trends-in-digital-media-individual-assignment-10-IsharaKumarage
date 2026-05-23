using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string promptMessage = "Press E to Interact";
    public UnityEvent onInteract = new UnityEvent();
    
    public void BaseInteract()
    {
        if (onInteract != null)
            onInteract.Invoke();
    }
}
