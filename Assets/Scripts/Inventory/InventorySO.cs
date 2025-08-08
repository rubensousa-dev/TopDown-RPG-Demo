using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "InventorySO", menuName = "Inventory/InventorySO")]
public class InventorySO : ScriptableObject
{
    [SerializeField]
    private List<InventoryItem> inventoryItems;

    [SerializeField]
    private int size = 10;
    public int Size => size;

    public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

    public void Initialize()
    {
        inventoryItems = new List<InventoryItem>();
        for (int i = 0; i < Size; i++)
        { 
            inventoryItems.Add(InventoryItem.GetEmptyItem());
        }
    }

    private int AddItemToFirstFreeSlot(ItemSO item, int quantity)
    {
        InventoryItem newItem = new InventoryItem
        {
            item = item,
            quantity = quantity
        };

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
            {
                inventoryItems[i] = newItem;
                return quantity;
            }
        }
        return 0;
    }
   
    public int AddItem(ItemSO item, int quantity)
    {
        if (!item.IsStackable)
        {
            while (quantity > 0 && !IsInventoryFull())
            {
                quantity -= AddItemToFirstFreeSlot(item, 1);
            }
            InformAboutChange();
            return quantity;
        }

        quantity = AddStackableItem(item, quantity);
        InformAboutChange();
        return quantity;
    }

    private bool IsInventoryFull() => !inventoryItems.Any(item => item.IsEmpty);

    public bool CanAddItem(ItemSO item, int quantity)
    {
        if (!item.IsStackable)
        {
            int emptySlots = inventoryItems.Count(slot => slot.IsEmpty);
            return emptySlots >= quantity;
        }

        // Para itens stackable, verificar se há espaço suficiente
        int totalSpace = 0;
        
        // Verificar slots existentes com o mesmo item
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (!inventoryItems[i].IsEmpty && inventoryItems[i].item.ID == item.ID)
            {
                totalSpace += item.MaxStackSize - inventoryItems[i].quantity;
            }
        }
        
        // Adicionar slots vazios
        int availableEmptySlots = inventoryItems.Count(slot => slot.IsEmpty);
        totalSpace += availableEmptySlots * item.MaxStackSize;
        
        return totalSpace >= quantity;
    }

    private int AddStackableItem(ItemSO item, int quantity)
    {
        // Primeiro, tentar adicionar aos slots existentes com o mesmo item
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty) continue;

            if (inventoryItems[i].item.ID == item.ID)
            {
                int maxStack = inventoryItems[i].item.MaxStackSize;
                int availableSpace = maxStack - inventoryItems[i].quantity;

                if (quantity > availableSpace)
                {
                    inventoryItems[i] = inventoryItems[i].ChangeQuantity(maxStack);
                    quantity -= availableSpace;
                }
                else
                {
                    inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].quantity + quantity);
                    return 0;
                }
            }
        }

        // Se ainda há quantidade para adicionar, criar novos slots
        while (quantity > 0 && !IsInventoryFull())
        {
            int addQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
            quantity -= addQuantity;
            AddItemToFirstFreeSlot(item, addQuantity);
        }

        return quantity;
    }

    public bool RemoveItemType(ItemSO item, int amount)
    {
        int remainingToRemove = amount;
        
        // Remover dos slots que contêm este item
        for (int i = inventoryItems.Count - 1; i >= 0; i--)
        {
            if (remainingToRemove <= 0) break;
            
            if (!inventoryItems[i].IsEmpty && inventoryItems[i].item.ID == item.ID)
            {
                int toRemove = Mathf.Min(remainingToRemove, inventoryItems[i].quantity);
                remainingToRemove -= toRemove;
                
                int newQuantity = inventoryItems[i].quantity - toRemove;
                if (newQuantity <= 0)
                {
                    inventoryItems[i] = InventoryItem.GetEmptyItem();
                }
                else
                {
                    inventoryItems[i] = inventoryItems[i].ChangeQuantity(newQuantity);
                }
            }
        }
        
        if (remainingToRemove > 0)
        {
            // Não foi possível remover toda a quantidade solicitada
            return false;
        }
        
        InformAboutChange();
        return true;
    }

    public void RemoveItem(int itemIndex, int amount)
    {
        if (itemIndex >= inventoryItems.Count) return;

        if (inventoryItems[itemIndex].IsEmpty) return;

        int remainder = inventoryItems[itemIndex].quantity - amount;
        if (remainder <= 0)
            inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
        else
            inventoryItems[itemIndex] = inventoryItems[itemIndex].ChangeQuantity(remainder);

        InformAboutChange();
    }

    public void AddItemStuff(ItemSO item, int quantity)
    {
        AddItem(item, quantity);
    }
    
    public void AddItem(InventoryItem item)
    {
        AddItemStuff(item.item, item.quantity);
    }

    public int GetItemCount(ItemSO item)
    {
        int totalCount = 0;
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (!inventoryItems[i].IsEmpty && inventoryItems[i].item.ID == item.ID)
            {
                totalCount += inventoryItems[i].quantity;
            }
        }
        return totalCount;
    }

    public Dictionary<int, InventoryItem> GetCurrentInventoryState()
    {
        var state = new Dictionary<int, InventoryItem>();
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (!inventoryItems[i].IsEmpty)
            {
                state[i] = inventoryItems[i];
            }
        }
        return state;
    }

    public InventoryItem GetItemAt(int itemIndex)
    {
        return inventoryItems[itemIndex];
    }

    public void SwapItems(int index1, int index2)
    {
        (inventoryItems[index1], inventoryItems[index2]) = (inventoryItems[index2], inventoryItems[index1]);
        InformAboutChange();
    }

    public void SetItemAtSlot(int slotIndex, ItemSO item, int quantity)
    {
        if (slotIndex < 0 || slotIndex >= inventoryItems.Count) return;
        
        if (item == null || quantity <= 0)
        {
            inventoryItems[slotIndex] = InventoryItem.GetEmptyItem();
        }
        else
        {
            inventoryItems[slotIndex] = new InventoryItem
            {
                item = item,
                quantity = quantity
            };
        }
        
        InformAboutChange();
    }

    private void InformAboutChange()
    {
        OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
    }
}

[Serializable]
public struct InventoryItem
{
    public int quantity;
    public ItemSO item;
   // public List<ItemParameter> itemState;

    public bool IsEmpty => item == null;

    public InventoryItem ChangeQuantity(int newQuantity)
    {
        return new InventoryItem
        {
            item = this.item,
            quantity = newQuantity,
            //itemState = new List<ItemParameter>(this.itemState)
        };
    }

    public static InventoryItem GetEmptyItem() => new InventoryItem
    {
        item = null,
        quantity = 0,
        //itemState = new List<ItemParameter>()
    };
}
