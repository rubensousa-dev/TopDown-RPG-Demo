using UnityEngine;

public class AppleInteraction : PickupItem
{
    [Header("Apple Specific Settings")]
    [SerializeField] private bool giveMultipleApples = false;
    [SerializeField] private int appleQuantity = 3; // Quantidade de maçãs que este item dá

    private void Start()
    {
        // Configurar quantidade específica para maçãs
        if (giveMultipleApples)
        {
            pickupQuantity = appleQuantity;
        }
    }
}
