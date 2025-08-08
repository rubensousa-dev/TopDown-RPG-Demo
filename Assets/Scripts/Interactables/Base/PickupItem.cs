using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public ItemSO itemData;

    [Header("Visual Effects")]
    public GameObject interactionUIPrompt;

    [Header("Interaction Settings")]
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] public int pickupQuantity = 1;

    public bool Interact(PlayerInteractor interactor)
    {
        if (itemData == null) return false;

        // Adicionar item ao inventário usando o singleton
        if (UIManager.Instance != null)
        {
            bool success = UIManager.Instance.AddItemToInventory(itemData, pickupQuantity);
            
            if (success)
            {
                // Item foi adicionado com sucesso
                Debug.Log($"Item '{itemData.ItemName}' adicionado ao inventário!");
                
                if (destroyOnPickup) 
                {
                    Destroy(gameObject);
                }
                return true;
            }
            else
            {
                // Inventário cheio
                Debug.Log($"Inventário cheio! Não foi possível adicionar {pickupQuantity} {itemData.ItemName}(s)");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("UIManager não encontrado! Item não pode ser adicionado ao inventário.");
            if (destroyOnPickup) Destroy(gameObject);
            return true;
        }
    }

    public bool IsInteractable()
    {
        return itemData != null;
    }

    public void SetHighlight(bool active)
    {
        if (interactionUIPrompt != null) interactionUIPrompt.SetActive(active);
    }
}