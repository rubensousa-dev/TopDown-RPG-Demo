using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private InventoryPage inventoryUI;
    [SerializeField] private DialogSystem dialogUI;

    [SerializeField] private int inventorySize = 10;
    
    private void Start()
    {
        inventoryUI.InitializeInventory(inventorySize);
    }

    private void Update()
    {
        // Só permitir abrir inventário se não há diálogo ativo
        if (InputManager.InventoryAction && (DialogSystem.Instance == null || !DialogSystem.Instance.IsDialogActive()))
        {
            if (!inventoryUI.isActiveAndEnabled)
                inventoryUI.Show();
            else
                inventoryUI.Hide();
        }
    }
}
