using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public ItemSO itemData;

    [Header("Visual Effects")]
    public GameObject glowEffect;
    public GameObject interactionUIPrompt;

    [Header("Interaction Settings")]
    [SerializeField] private bool destroyOnPickup = true;

    public bool Interact(PlayerInteractor interactor)
    {
        if (itemData == null) return false;
        if (destroyOnPickup) Destroy(gameObject);
        return true;
    }

    public bool IsInteractable()
    {
        return itemData != null;
    }

    public void SetHighlight(bool active)
    {
        if (glowEffect != null) glowEffect.SetActive(active);

        if (interactionUIPrompt != null) interactionUIPrompt.SetActive(active);
    }
}