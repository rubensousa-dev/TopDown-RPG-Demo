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
        if (UIManager.Instance != null)
        {
            bool success = UIManager.Instance.AddItemToInventory(itemData, pickupQuantity);
            
            if (success)
            {
                if (destroyOnPickup) 
                {
                    Destroy(gameObject);
                }
                return true;
            }
            else return false;
            
        }
        else
        {
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