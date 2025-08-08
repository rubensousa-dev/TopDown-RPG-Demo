using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryItemSlot : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
{
    [SerializeField] public Image itemImage;
    [SerializeField] public TMP_Text quantityText;
    [SerializeField] private Image borderImage;

    public event Action<InventoryItemSlot> OnItemClicked;
    public event Action<InventoryItemSlot> OnItemDroppedOn;
    public event Action<InventoryItemSlot> OnItemBeginDrag;
    public event Action<InventoryItemSlot> OnItemEndDrag;
    public event Action<InventoryItemSlot> OnRightMouseBtnClick;

    private bool empty = true;

    private void Awake()
    {
        Reset();
        Deselect();
    }

    public void Reset()
    {
        itemImage.gameObject.SetActive(false);
        quantityText.text = string.Empty;
        empty = true;
    }

    public void Set(Sprite sprite, int quantity)
    {
        itemImage.sprite = sprite;
        itemImage.gameObject.SetActive(true);
        quantityText.text = quantity.ToString();
        empty = false;
    }

    public void Select() => borderImage.enabled = true;

    public void Deselect() => borderImage.enabled = false;
 

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnItemClicked?.Invoke(this);
        else
            OnRightMouseBtnClick?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!empty)
            OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnItemEndDrag?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnItemDroppedOn?.Invoke(this);
    }
}
