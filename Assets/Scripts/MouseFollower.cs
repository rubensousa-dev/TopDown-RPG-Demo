using UnityEngine;

public class UIDraggedItem : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    //[SerializeField] private InventoryItem item;

    private void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
       // item = GetComponentInChildren<InventoryItem>();
    }

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 localPos
        );

        transform.position = canvas.transform.TransformPoint(localPos);
    }

    public void SetData(Sprite sprite, int quantity)
    {
        //item.SetData(sprite, quantity);
    }

    public void Toggle(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
