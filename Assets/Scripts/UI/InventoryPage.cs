using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private InventoryItemSlot slotPrefab;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private InventoryDescription inventoryItemDescription;
    [SerializeField] private MouseFollower mouseFollower;

    private readonly List<InventoryItemSlot> itemSlots = new();
    private int? currentlyDraggedItemIndex = null;
    private int? lastSelectedItemIndex = null;

    public List<InventoryItemSlot> ItemSlots => itemSlots;
    public int? LastSelectedItemIndex => lastSelectedItemIndex;

    public event Action<int> OnDescriptionRequested;
    public event Action<int> OnItemActionRequested;
    public event Action<int> OnStartDragging;
    public event Action<int, int> OnSwapItems;
    public event Action<int> OnUseItemRequested;
    public event Action<int> OnDropItemRequested;

    private void Awake()
    {
        Hide();
        mouseFollower.Toggle(false);
        inventoryItemDescription.Clear();
        if (inventoryItemDescription != null)
        {
            inventoryItemDescription.OnUseButtonClicked += OnUseButtonClicked;
            inventoryItemDescription.OnDropButtonClicked += OnDropButtonClicked;
        }
    }

    public void InitializeInventory(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            var slot = Instantiate(slotPrefab, contentPanel);
            itemSlots.Add(slot);

            slot.OnItemClicked += OnItemSelected;
            slot.OnItemBeginDrag += OnBeginDrag;
            slot.OnItemDroppedOn += OnItemSwapped;
            slot.OnItemEndDrag += OnEndDrag;
            slot.OnRightMouseBtnClick += OnRightClick;
        }
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        if (itemIndex < 0 || itemIndex >= itemSlots.Count) return;
        itemSlots[itemIndex].Set(itemImage, itemQuantity);
    }

    public void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
    {
        if (itemIndex < 0 || itemIndex >= itemSlots.Count) return;
        string fullDescription = description;
        if (itemSlots[itemIndex].quantityText.text != string.Empty)
        {
            int quantity = int.Parse(itemSlots[itemIndex].quantityText.text);
            if (quantity > 1)
            {
                fullDescription += $"\n\nQuantity: {quantity}";
            }
        }

        inventoryItemDescription.Set(itemImage, name, fullDescription);
        DeselectAllItems();
        itemSlots[itemIndex].Select();
        lastSelectedItemIndex = itemIndex;
    }

    public void UpdateDescriptionWithoutReset(int itemIndex, Sprite itemImage, string name, string description)
    {
        if (itemIndex < 0 || itemIndex >= itemSlots.Count) return;

        inventoryItemDescription.Set(itemImage, name, description);
        lastSelectedItemIndex = itemIndex;
        DeselectAllItems();
        itemSlots[itemIndex].Select();
    }

    public void ResetAllItems()
    {
        foreach (var slot in itemSlots)
        {
            slot.Reset();
            slot.Deselect();
        }
        if (lastSelectedItemIndex.HasValue && lastSelectedItemIndex.Value < itemSlots.Count)
        {
            itemSlots[lastSelectedItemIndex.Value].Select();
        }
    }

    public void ResetSelection()
    {
        inventoryItemDescription.Clear();
        DeselectAllItems();
        lastSelectedItemIndex = null;
    }

    private void OnItemSelected(InventoryItemSlot slot)
    {
        int index = GetItemIndex(slot);
        if (index == -1) return;
        
        lastSelectedItemIndex = index;
        OnDescriptionRequested?.Invoke(index);
    }

    private void OnBeginDrag(InventoryItemSlot slot)
    {
        int index = GetItemIndex(slot);
        if (index == -1) return;

        currentlyDraggedItemIndex = index;
        OnItemSelected(slot);
        OnStartDragging?.Invoke(index);
    }

    private void OnEndDrag(InventoryItemSlot slot)
    {
        ResetDraggedItem();
    }

    private void OnItemSwapped(InventoryItemSlot slot)
    {
        int index = GetItemIndex(slot);
        if (index == -1 || currentlyDraggedItemIndex == null) return;

        OnSwapItems?.Invoke(currentlyDraggedItemIndex.Value, index);
        
        if (currentlyDraggedItemIndex.HasValue)
        {
            lastSelectedItemIndex = index; 
            DeselectAllItems();
            itemSlots[index].Select();
        }
    }

    private void OnRightClick(InventoryItemSlot slot)
    {
        int index = GetItemIndex(slot);
        OnItemActionRequested?.Invoke(index);
    }

    private void OnUseButtonClicked()
    {
        if (lastSelectedItemIndex.HasValue)
        {
            OnUseItemRequested?.Invoke(lastSelectedItemIndex.Value);
        }
    }

    private void OnDropButtonClicked()
    {
        if (lastSelectedItemIndex.HasValue)
        {
            OnDropItemRequested?.Invoke(lastSelectedItemIndex.Value);
        }
    }

    public void CreateDraggedItem(Sprite sprite, int quantity)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(sprite, quantity);
    }

    private void ResetDraggedItem()
    {
        mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = null;
    }

    private void DeselectAllItems()
    {
        foreach (var slot in itemSlots)
            slot.Deselect();
    }

    private int GetItemIndex(InventoryItemSlot slot)
    {
        return itemSlots.IndexOf(slot);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ResetSelection();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        ResetDraggedItem();
    }
}
