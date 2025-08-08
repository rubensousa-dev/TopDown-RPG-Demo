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
        // Obter referências se não foram atribuídas
        if (playerSprite == null)
            playerSprite = GetComponent<SpriteRenderer>();
        
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
            
        // Salvar valores originais
        if (playerSprite != null)
            originalColor = playerSprite.color;
            
        if (playerMovement != null)
            originalSpeed = playerMovement.moveSpeed;
    }

    // Método público para usar item
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
                // Maçã não tem efeito especial
                Debug.Log("Você comeu uma maçã. Deliciosa!");
                break;
            default:
                Debug.Log($"Item '{item.ItemName}' usado, mas não tem efeito especial.");
                break;
        }
    }
    
    private void ApplyGreenPotionEffect()
    {
        Debug.Log("🍃 Efeito da Poção Verde ativado!");
        
        // Parar efeito atual se houver
        if (currentEffectCoroutine != null)
            StopCoroutine(currentEffectCoroutine);
            
        // Aplicar efeito verde
        currentEffectCoroutine = StartCoroutine(GreenPotionEffect());
    }
    
    private void ApplyBluePotionEffect()
    {
        Debug.Log("💙 Efeito da Poção Azul ativado!");
        
        // Parar efeito atual se houver
        if (currentEffectCoroutine != null)
            StopCoroutine(currentEffectCoroutine);
            
        // Aplicar efeito azul
        currentEffectCoroutine = StartCoroutine(BluePotionEffect());
    }
    
    private IEnumerator GreenPotionEffect()
    {
        // Mudar cor para verde
        if (playerSprite != null)
            playerSprite.color = greenPotionColor;
            
        // Aguardar duração do efeito
        yield return new WaitForSeconds(effectDuration);
        
        // Restaurar cor original
        if (playerSprite != null)
            playerSprite.color = originalColor;
            
        Debug.Log("🍃 Efeito da Poção Verde terminou!");
        currentEffectCoroutine = null;
    }
    
    private IEnumerator BluePotionEffect()
    {
        // Mudar cor para azul
        if (playerSprite != null)
            playerSprite.color = bluePotionColor;
            
        // Aumentar velocidade
        if (playerMovement != null)
            playerMovement.moveSpeed = originalSpeed * speedMultiplier;
            
        // Aguardar duração do efeito
        yield return new WaitForSeconds(effectDuration);
        
        // Restaurar cor e velocidade originais
        if (playerSprite != null)
            playerSprite.color = originalColor;
            
        if (playerMovement != null)
            playerMovement.moveSpeed = originalSpeed;
            
        Debug.Log("💙 Efeito da Poção Azul terminou!");
        currentEffectCoroutine = null;
    }
    
    // Método para cancelar efeitos ativos (útil para debugging)
    public void CancelAllEffects()
    {
        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
            currentEffectCoroutine = null;
        }
        
        // Restaurar valores originais
        if (playerSprite != null)
            playerSprite.color = originalColor;
            
        if (playerMovement != null)
            playerMovement.moveSpeed = originalSpeed;
            
        Debug.Log("Efeitos cancelados!");
    }
}
