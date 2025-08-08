using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventorySaveSystem : MonoBehaviour
{
    [Header("Save Settings")]
    [SerializeField] private bool autoSave = true;
    [SerializeField] private float autoSaveInterval = 30f;
    
    private InventorySO inventoryData;
    private float lastSaveTime;
    
    private List<SavedInventoryItem> savedItems = new List<SavedInventoryItem>();
    
    private const string INVENTORY_ITEMS_COUNT_KEY = "InventoryItemsCount";
    private const string INVENTORY_ITEM_KEY = "InventoryItem_{0}";
    
    [System.Serializable]
    public class SavedInventoryItem
    {
        public int slotIndex;
        public int itemIndex;
        public int quantity;
        
        public SavedInventoryItem(int slot, int item, int qty)
        {
            slotIndex = slot;
            itemIndex = item;
            quantity = qty;
        }
    }
    
    private void Start()
    {
        inventoryData = FindFirstObjectByType<UIManager>()?.GetInventory();
        
        if (inventoryData != null)
        {
            inventoryData.OnInventoryUpdated += OnInventoryChanged;
        }
    }
    
    private void Update()
    {
        if (autoSave && Time.time - lastSaveTime > autoSaveInterval)
        {
            SaveInventory();
            lastSaveTime = Time.time;
        }
    }
    
    private void OnInventoryChanged(Dictionary<int, InventoryItem> state)
    {
        UpdateSavedItemsList(state);
        
        if (autoSave)
        {
            SaveInventory();
        }
    }
    
    private void UpdateSavedItemsList(Dictionary<int, InventoryItem> state)
    {
        savedItems.Clear();
        
        foreach (var item in state)
        {
            if (!item.Value.IsEmpty)
            {
                savedItems.Add(new SavedInventoryItem(
                    item.Key, 
                    item.Value.item.ItemIndex, 
                    item.Value.quantity
                ));
            }
        }
    }
    
    public void SaveInventory()
    {
        if (inventoryData == null) return;
        
        try
        {
            var state = inventoryData.GetCurrentInventoryState();
            UpdateSavedItemsList(state);

            PlayerPrefs.SetInt(INVENTORY_ITEMS_COUNT_KEY, savedItems.Count);
            
            for (int i = 0; i < savedItems.Count; i++)
            {
                var savedItem = savedItems[i];
                string itemKey = string.Format(INVENTORY_ITEM_KEY, i);
                
                string json = JsonUtility.ToJson(savedItem);
                PlayerPrefs.SetString(itemKey, json);
            }
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
        }
    }
    
    public bool LoadInventory()
    {
        if (inventoryData == null) return false;
        
        try
        {
            if (!PlayerPrefs.HasKey(INVENTORY_ITEMS_COUNT_KEY)) return false;
         
            
            inventoryData.Initialize();
            
            int itemsCount = PlayerPrefs.GetInt(INVENTORY_ITEMS_COUNT_KEY, 0);
            
            int loadedItems = 0;
            
            for (int i = 0; i < itemsCount; i++)
            {
                string itemKey = string.Format(INVENTORY_ITEM_KEY, i);
                string json = PlayerPrefs.GetString(itemKey, "");
                
                if (!string.IsNullOrEmpty(json))
                {
                    SavedInventoryItem savedItem = JsonUtility.FromJson<SavedInventoryItem>(json);
                    
                    ItemSO itemSO = FindItemSOByIndex(savedItem.itemIndex);
                    if (itemSO != null)
                    {
                        inventoryData.SetItemAtSlot(savedItem.slotIndex, itemSO, savedItem.quantity);
                        loadedItems++;
                    }
                }
            }
            
            return loadedItems > 0;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    private ItemSO FindItemSOByIndex(int itemIndex)
    {
        ItemSO foundItem = null;
        
        #if UNITY_EDITOR
        string[] guids = AssetDatabase.FindAssets("t:ItemSO", new string[] { "Assets/Scripts/SO" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemSO item = AssetDatabase.LoadAssetAtPath<ItemSO>(path);
            
            if (item != null && item.ItemIndex == itemIndex)
            {
                foundItem = item;
                break;
            }
        }
        #endif
        
        return foundItem;
    }
    
    public void ClearSaveData()
    {
        int itemsCount = PlayerPrefs.GetInt(INVENTORY_ITEMS_COUNT_KEY, 0);
        
        PlayerPrefs.DeleteKey(INVENTORY_ITEMS_COUNT_KEY);
        
        for (int i = 0; i < itemsCount; i++)
        {
            PlayerPrefs.DeleteKey(string.Format(INVENTORY_ITEM_KEY, i));
        }
        
        PlayerPrefs.Save();
    }


    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && autoSave) SaveInventory();
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && autoSave) SaveInventory();
    }
    
    private void OnApplicationQuit()
    {
        if (autoSave) SaveInventory();
    }
}
