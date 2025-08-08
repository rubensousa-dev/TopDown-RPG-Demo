using UnityEngine;

public class AppleInteraction : PickupItem
{
    [Header("Apple Specific Settings")]
    [SerializeField] private bool giveMultipleApples = false;
    [SerializeField] private int appleQuantity = 3;

    private void Start()
    {
        if (giveMultipleApples)
        {
            pickupQuantity = appleQuantity;
        }
    }
}
