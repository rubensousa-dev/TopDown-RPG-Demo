using UnityEngine;

public class DialogueInteraction : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    public DialogueSO dialogueData;

    [Header("Visual Effects")]
    public GameObject interactionUIPrompt;

    [Header("Interaction Settings")]
    [SerializeField] private bool canInteractMultipleTimes = true;
    [SerializeField] private bool disableAfterInteraction = false;

    private bool hasInteracted = false;

    public virtual bool Interact(PlayerInteractor interactor)
    {
        if (dialogueData == null)
        {
            return false;
        }

        if (!canInteractMultipleTimes && hasInteracted)
        {
            return false;
        }


        hasInteracted = true;

        if (disableAfterInteraction)
        {
            SetHighlight(false);
            enabled = false;
        }

        return true;
    }

    public bool IsInteractable()
    {
        if (dialogueData == null) return false;
        if (!canInteractMultipleTimes && hasInteracted) return false;
        return true;
    }

    public void SetHighlight(bool active)
    {
        if (interactionUIPrompt != null)
            interactionUIPrompt.SetActive(active);
    }
}
