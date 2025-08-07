using UnityEngine;

public class NPCInteraction : DialogueInteraction
{
    [Header("NPC Specific")]
    [SerializeField] private float cooldownTime = 2f;
    private float lastInteractionTime = 0f;
    
    public override bool Interact(PlayerInteractor interactor)
    {
        if (Time.time - lastInteractionTime < cooldownTime)
        {
            return false;
        }
        
        if (DialogSystem.Instance != null && DialogSystem.Instance.IsDialogActive())
        {
            return false;
        }
        
        lastInteractionTime = Time.time;
        return base.Interact(interactor);
    }
    
    public override bool IsInteractable()
    {
        if (Time.time - lastInteractionTime < cooldownTime)
            return false;
            
        return base.IsInteractable();
    }
}
