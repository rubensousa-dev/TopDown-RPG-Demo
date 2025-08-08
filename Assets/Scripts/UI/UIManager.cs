using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private InventoryPage inventoryUI;
    [SerializeField] private DialogSystem dialogUI;
    [SerializeField] private InventorySO inventoryData;
    
    public List<InventoryItem> initialItems =new List<InventoryItem>();
       public static UIManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        PrepareUI();
        PrepareInventoryData();
    }

    public InventorySO GetInventory()
    {
        return inventoryData;
    }
    public bool AddItemToInventory(ItemSO item, int quantity)
    {
        if (inventoryData == null) return false;
        
        int remaining = inventoryData.AddItem(item, quantity);
        bool success = remaining == 0;
        
        if (success)
        {
            ShowItemCollectedNotification(item, quantity);
        }
        
        return success;
    }

    private void ShowItemCollectedNotification(ItemSO item, int quantity)
    {
        string message = quantity > 1 ? $"+{quantity} {item.ItemName}s" : $"+1 {item.ItemName}";
        int totalCount = inventoryData.GetItemCount(item);
    }

    private void PrepareInventoryData()
    {
        InventorySaveSystem saveSystem = FindFirstObjectByType<InventorySaveSystem>();
        bool loadedFromSave = false;
        
        if (saveSystem != null)
        {
            loadedFromSave = saveSystem.LoadInventory();
        }
        
        if (!loadedFromSave)
        {
            inventoryData.Initialize();
            foreach (InventoryItem item in initialItems)
            {
                if (item.IsEmpty) continue;
                inventoryData.AddItem(item);
            }
        }
        
        inventoryData.OnInventoryUpdated += UpdateUI;
    }

    private void UpdateUI(Dictionary<int, InventoryItem> state)
    {
        int? currentSelectedIndex = inventoryUI.LastSelectedItemIndex;
        
        inventoryUI.ResetAllItems();
        foreach (var item in state)
        {
            inventoryUI.UpdateData(item.Key, item.Value.item.Icon, item.Value.quantity);
        }
        
        if (currentSelectedIndex.HasValue && currentSelectedIndex.Value < state.Count)
        {
            if (state.ContainsKey(currentSelectedIndex.Value))
            {
                InventoryItem selectedItem = state[currentSelectedIndex.Value];
                if (!selectedItem.IsEmpty)
                {
                    inventoryUI.UpdateDescription(currentSelectedIndex.Value, selectedItem.item.Icon, selectedItem.item.ItemName, selectedItem.item.Description);
                }
            }
        }
    }

    private void PrepareUI()
    {
        inventoryUI.InitializeInventory(inventoryData.Size);
        this.inventoryUI.OnDescriptionRequested +=  HandleUpdateDescription;
        this.inventoryUI.OnSwapItems += HandleSwipe;
        this.inventoryUI.OnStartDragging += HandleStartDraggin;
        this.inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        this.inventoryUI.OnUseItemRequested += HandleUseItemRequest;
        this.inventoryUI.OnDropItemRequested += HandleDropItemRequest;
    }

    private void HandleItemActionRequest(int item)
    {
        InventoryItem itemData = inventoryData.GetItemAt(item);
        if (itemData.IsEmpty) return;
        UseItemFromInventory(itemData.item);
        inventoryData.RemoveItem(item, 1);
    }
    
    private void HandleUseItemRequest(int item)
    {
        InventoryItem itemData = inventoryData.GetItemAt(item);
        if (itemData.IsEmpty) return;
        UseItemFromInventory(itemData.item);
        inventoryData.RemoveItem(item, 1);
        inventoryUI.Hide();
    }
    
    private void HandleDropItemRequest(int item)
    {
        InventoryItem itemData = inventoryData.GetItemAt(item);
        if (itemData.IsEmpty) return;
        inventoryData.RemoveItem(item, itemData.quantity);
    }
    
    private void UseItemFromInventory(ItemSO item)
    {
        PlayerModification playerMod = FindFirstObjectByType<PlayerModification>();
        
        if (playerMod != null)
        {
            playerMod.UseItem(item);
        }
    }

    private void HandleStartDraggin(int item)
    {
        InventoryItem itemData = inventoryData.GetItemAt(item);
        if (itemData.IsEmpty)
        {
            inventoryUI.ResetSelection();
            return;
        }
        inventoryUI.CreateDraggedItem(itemData.item.Icon, itemData.quantity);

    }

    private void HandleSwipe(int item_indexOne, int item_indexTwo)
    {
        inventoryData.SwapItems(item_indexOne, item_indexTwo);
        
        InventoryItem movedItem = inventoryData.GetItemAt(item_indexTwo);
        if (!movedItem.IsEmpty)
        {
            ItemSO itemSO = movedItem.item;
            inventoryUI.UpdateDescriptionWithoutReset(item_indexTwo, itemSO.Icon, itemSO.ItemName, itemSO.Description);
        }
    }

    private void HandleUpdateDescription(int item)
    {
        InventoryItem itemData = inventoryData.GetItemAt(item);
        if (itemData.IsEmpty)
        {
            inventoryUI.ResetSelection();
            return;
        }
        ItemSO itemSO = itemData.item;
        inventoryUI.UpdateDescription(item, itemSO.Icon, itemSO.ItemName, itemSO.Description);
    }

    private void Update()
    {

        if (InputManager.InventoryAction && !inventoryUI.isActiveAndEnabled && (DialogSystem.Instance == null || !DialogSystem.Instance.IsDialogActive()))
        {  
            inventoryUI.Show();

            foreach (var item in inventoryData.GetCurrentInventoryState())
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.Icon, item.Value.quantity);

            }
        }else if (InputManager.InventoryAction && inventoryUI.isActiveAndEnabled && (DialogSystem.Instance == null || !DialogSystem.Instance.IsDialogActive()))
        {
            inventoryUI.Hide();
        }

      
    }
}
