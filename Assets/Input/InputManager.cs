using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    public static Vector2 MoveInput;
    private InputAction moveAction;
    private PlayerInput playerInput;

    private void Awake()
    {
       playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }


    // Update is called once per frame
    void Update()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
    }
}
