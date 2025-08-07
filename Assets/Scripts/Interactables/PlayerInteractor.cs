using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 1.2f;
    [SerializeField] private Vector2 interactionOffset = new Vector2(0f, 0.5f);
    [SerializeField] private LayerMask interactableLayer;

    private IInteractable currentInteractable;

    private void Update()
    {
        CheckForInteractables();

        if (InputManager.InteractInput && currentInteractable != null)
        {
            currentInteractable.Interact(this);
        }
    }

    private void CheckForInteractables()
    {
        Vector2 origin = (Vector2)transform.position + interactionOffset;

        Collider2D hit = Physics2D.OverlapCircle(origin, interactionRadius, interactableLayer);

        if (hit != null)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();

            if (interactable != null && interactable.IsInteractable())
            {
                if (currentInteractable != interactable)
                {
                    ClearHighlight();
                    currentInteractable = interactable;
                    currentInteractable.SetHighlight(true);
                }
                return;
            }
        }

        ClearHighlight();
    }

    private void ClearHighlight()
    {
        if (currentInteractable != null)
        {
            currentInteractable.SetHighlight(false);
            currentInteractable = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 origin = (Vector2)transform.position + interactionOffset;
        Gizmos.DrawWireSphere(origin, interactionRadius);
    }
}
