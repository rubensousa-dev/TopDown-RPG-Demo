using UnityEngine;

public class ChestInteraction : DialogueInteraction
{
    [Header("Chest Specific")]
    [SerializeField] private float cooldownTime = 2f;
    private float lastInteractionTime = 0f;
    
    public override bool Interact(PlayerInteractor interactor)
    {
        if (Time.time - lastInteractionTime < cooldownTime)
        {
            return false;
        }
        
        lastInteractionTime = Time.time;
        return base.Interact(interactor);
    }
}
