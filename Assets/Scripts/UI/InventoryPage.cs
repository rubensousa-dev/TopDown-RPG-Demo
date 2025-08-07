using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{
    [SerializeField] private InventoryItemSlot slotPrefab;
    [SerializeField] private RectTransform contentPanel;

    private readonly List<InventoryItemSlot> itemSlots = new();

    public void InitializeInventory(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            var slot = Instantiate(slotPrefab, contentPanel);
            itemSlots.Add(slot);

            slot.OnItemClicked += HandleItemSelected;
            slot.OnItemBeginDrag += HandleBeginDrag;
            slot.OnItemDroppedOn += HandleItemSwapped;
            slot.OnItemEndDrag += HandleEndDrag;
            slot.OnRightMouseBtnClick += HandleRightClick;
        }
    }

    private void HandleItemSelected(InventoryItemSlot slot)
    {
        Debug.Log(slot.name);
    }

    private void HandleBeginDrag(InventoryItemSlot slot)
    {
        Debug.Log(slot.name);
    }

    private void HandleEndDrag(InventoryItemSlot slot)
    {
        Debug.Log(slot.name);
    }

    private void HandleItemSwapped(InventoryItemSlot slot)
    {
        Debug.Log(slot.name);
    }

    private void HandleRightClick(InventoryItemSlot slot)
    {
        Debug.Log(slot.name);
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}

