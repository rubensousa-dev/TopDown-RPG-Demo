using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    public static Vector2 MoveInput;
    public static bool InteractInput, InventoryAction;
    
    private InputAction moveAction, interactAction,inventoryAction;
    private PlayerInput playerInput;
   

    private void Awake()
    {
       playerInput = GetComponent<PlayerInput>();
       moveAction = playerInput.actions["Move"];
       interactAction = playerInput.actions["Interact"];
       inventoryAction = playerInput.actions["Inventory"];
    }

    void Update()
    {
        bool dialogActive = DialogSystem.Instance != null && DialogSystem.Instance.IsDialogActive();
        
        if (dialogActive)
        {
            MoveInput = Vector2.zero;
            InteractInput = false;
            InventoryAction = false;
        }
        else
        {
            MoveInput = moveAction.ReadValue<Vector2>();
            InteractInput = interactAction.WasPressedThisFrame();
            InventoryAction = inventoryAction.WasPressedThisFrame();
        }
    }
}
