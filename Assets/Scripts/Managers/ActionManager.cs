
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public UnityEvent jumpCheck;
    public UnityEvent<int> moveCheck;
    public UnityEvent interact;
    public UnityEvent attack;
    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpCheck.Invoke();
        }
    }

    public void OnMoveAction(InputAction.CallbackContext context)
    {
        // Debug.Log("OnMoveAction callback invoked");
        if (context.started)
        {
            int faceRight = context.ReadValue<Vector2>().x > 0 ? 1 : -1;
            moveCheck.Invoke(faceRight);
        }
        if (context.canceled)
        {
            moveCheck.Invoke(0);
        }

    }

    public void OnAttackAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            attack.Invoke();
        }
    }

    public void OnInteractAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            interact.Invoke();
        }
    }
}