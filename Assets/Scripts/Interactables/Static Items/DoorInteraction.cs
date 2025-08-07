using UnityEngine;

public class DoorInteraction : DialogueInteraction
{
    [Header("Door Specific")]
    [SerializeField] private float cooldownTime = 2f;
    private float lastInteractionTime = 0f;
    
    public override bool Interact(PlayerInteractor interactor)
    {
        // Check the cooldown
        if (Time.time - lastInteractionTime < cooldownTime)
        {
            return false;
        }
        
        lastInteractionTime = Time.time;
        return base.Interact(interactor);
    }
}
