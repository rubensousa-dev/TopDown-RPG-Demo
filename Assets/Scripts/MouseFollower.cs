using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private InventoryItemSlot item;

    private void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
        item = GetComponentInChildren<InventoryItemSlot>();
    }

    private void Update()
    {
        // Convert the mouse screen position to a local position relative to the canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,      // Target canvas (as RectTransform)
            Input.mousePosition,                  // Current mouse position on screen
            canvas.worldCamera,                   // Camera rendering the canvas
            out Vector2 localPos                  // Resulting local position inside the canvas
        );

        // Move the dragged item to follow the mouse within the canvas space
        transform.position = canvas.transform.TransformPoint(localPos);
    }


    public void SetData(Sprite sprite, int quantity)
    {
        item.Set(sprite, quantity);
    }

    public void Toggle(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
