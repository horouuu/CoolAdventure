using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpPressed;
    public static bool JumpReleased;
    public static bool JumpHeld;

    private InputAction _movement;
    private InputAction _jump;

    void Awake() {
        PlayerInput = GetComponent<PlayerInput>();

        _movement = PlayerInput.actions["Movement"];
        _jump = PlayerInput.actions["Jump"];

    }

    void Update()
    {
        
    }
}
