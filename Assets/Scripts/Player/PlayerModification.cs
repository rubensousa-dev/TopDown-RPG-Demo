using UnityEngine;
using System.Collections;

public class PlayerModification : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private PlayerMovement playerMovement;
    
    [Header("Effect Settings")]
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private float speedMultiplier = 2f;
    
    [Header("Potion Colors")]
    [SerializeField] private Color greenPotionColor = Color.green;
    [SerializeField] private Color bluePotionColor = Color.blue;
    
    private Color originalColor;
    private float originalSpeed;
    private Coroutine currentEffectCoroutine;
    
    void Start()
    {
        if (playerSprite == null)
            playerSprite = GetComponent<SpriteRenderer>();
        
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
            
        if (playerSprite != null)
            originalColor = playerSprite.color;
            
        if (playerMovement != null)
            originalSpeed = playerMovement.moveSpeed;
    }

    public void UseItem(ItemSO item)
    {
        if (item == null) return;
        
        switch (item.ItemName)
        {
            case "Green Potion":
                ApplyGreenPotionEffect();
                break;
            case "Blue Potion":
                ApplyBluePotionEffect();
                break;
            case "Apple":
                break;
            default:
                break;
        }
    }
    
    private void ApplyGreenPotionEffect()
    {
        if (currentEffectCoroutine != null)
            StopCoroutine(currentEffectCoroutine);
        currentEffectCoroutine = StartCoroutine(GreenPotionEffect());
    }
    
    private void ApplyBluePotionEffect()
    {

        if (currentEffectCoroutine != null)
            StopCoroutine(currentEffectCoroutine);
            
        currentEffectCoroutine = StartCoroutine(BluePotionEffect());
    }
    
    private IEnumerator GreenPotionEffect()
    {
        if (playerSprite != null)
            playerSprite.color = greenPotionColor;
            
        yield return new WaitForSeconds(effectDuration);

        if (playerSprite != null)
            playerSprite.color = originalColor;
        currentEffectCoroutine = null;
    }
    
    private IEnumerator BluePotionEffect()
    {
        if (playerSprite != null)
            playerSprite.color = bluePotionColor;

        if (playerMovement != null)
            playerMovement.moveSpeed = originalSpeed * speedMultiplier;

        yield return new WaitForSeconds(effectDuration);

        if (playerSprite != null)
            playerSprite.color = originalColor;
            
        if (playerMovement != null)
            playerMovement.moveSpeed = originalSpeed;
        currentEffectCoroutine = null;
    }
  
}
