using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable interactableDetected = null;
    public GameObject interactionIcon;

    void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void Interact()
    {
        interactableDetected?.Interact();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableDetected = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.TryGetComponent(out IInteractable interactable) && interactable == interactableDetected)
        {
            interactableDetected = null;
            interactionIcon.SetActive(false);
        }
    }
}
