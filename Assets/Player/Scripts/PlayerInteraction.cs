using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable interactableDetected = null;
    private InputMap playercontrols;
    private InputAction interact;
    public GameObject interactionIcon;

    void Awake()
    {
        playercontrols = new InputMap();
    }

    void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void OnInteract()
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

    void OnEnable()
    {
        interact = playercontrols.PlayerInputMap.Interact;
        interact.Enable();
    }

    void OnDisable()
    {
        interact.Disable();
    }

    void Update()
    {
        if (interact.triggered)
        {
            OnInteract();
        }
    }
}
