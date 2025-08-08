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
    
    // Lista para manter o estado atual do inventário
    private List<SavedInventoryItem> savedItems = new List<SavedInventoryItem>();
    
    // Chaves para PlayerPrefs
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
            Debug.Log("✅ InventorySaveSystem inicializado com sucesso!");
        }
        else
        {
            Debug.LogWarning("⚠️ UIManager não encontrado!");
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
        // Atualizar a lista com o estado atual
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
        
        Debug.Log($"📝 Lista atualizada: {savedItems.Count} itens");
    }
    
    public void SaveInventory()
    {
        if (inventoryData == null) return;
        
        try
        {
            var state = inventoryData.GetCurrentInventoryState();
            UpdateSavedItemsList(state);
            
            Debug.Log($"💾 Salvando {savedItems.Count} itens...");
            
            // Salvar número de itens
            PlayerPrefs.SetInt(INVENTORY_ITEMS_COUNT_KEY, savedItems.Count);
            
            // Salvar cada item
            for (int i = 0; i < savedItems.Count; i++)
            {
                var savedItem = savedItems[i];
                string itemKey = string.Format(INVENTORY_ITEM_KEY, i);
                
                // Converter para JSON
                string json = JsonUtility.ToJson(savedItem);
                PlayerPrefs.SetString(itemKey, json);
                
                Debug.Log($"  ✅ Item {i}: Slot {savedItem.slotIndex}, Item {savedItem.itemIndex}, Qty {savedItem.quantity}");
            }
            
            PlayerPrefs.Save();
            Debug.Log($"✅ Inventário salvo com sucesso! {savedItems.Count} itens.");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Erro ao salvar: {e.Message}");
        }
    }
    
    public bool LoadInventory()
    {
        if (inventoryData == null) return false;
        
        try
        {
            if (!PlayerPrefs.HasKey(INVENTORY_ITEMS_COUNT_KEY))
            {
                Debug.Log("ℹ️ Nenhum dado encontrado.");
                return false;
            }
            
            inventoryData.Initialize();
            
            int itemsCount = PlayerPrefs.GetInt(INVENTORY_ITEMS_COUNT_KEY, 0);
            Debug.Log($"🔄 Carregando {itemsCount} itens...");
            
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
                        Debug.Log($"  ✅ Slot {savedItem.slotIndex}: {savedItem.quantity}x {itemSO.ItemName}");
                    }
                }
            }
            
            Debug.Log($"✅ Carregados {loadedItems} itens com sucesso!");
            return loadedItems > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Erro ao carregar: {e.Message}");
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
        Debug.Log("🗑️ Dados apagados!");
    }
    
    public void PrintSaveInfo()
    {
        if (!PlayerPrefs.HasKey(INVENTORY_ITEMS_COUNT_KEY))
        {
            Debug.Log("ℹ️ Nenhum dado encontrado.");
            return;
        }
        
        int itemsCount = PlayerPrefs.GetInt(INVENTORY_ITEMS_COUNT_KEY, 0);
        Debug.Log($"📊 Dados salvos: {itemsCount} itens");
        
        for (int i = 0; i < itemsCount; i++)
        {
            string itemKey = string.Format(INVENTORY_ITEM_KEY, i);
            string json = PlayerPrefs.GetString(itemKey, "");
            
            if (!string.IsNullOrEmpty(json))
            {
                SavedInventoryItem savedItem = JsonUtility.FromJson<SavedInventoryItem>(json);
                ItemSO item = FindItemSOByIndex(savedItem.itemIndex);
                string itemName = item != null ? item.ItemName : $"Item {savedItem.itemIndex}";
                
                Debug.Log($"  - Slot {savedItem.slotIndex}: {savedItem.quantity}x {itemName}");
            }
        }
    }
    
    public void TestSaveLoadProblem()
    {
        Debug.Log("🔍 Testando novo sistema...");
        
        var currentState = inventoryData.GetCurrentInventoryState();
        Debug.Log($"📊 Estado atual: {currentState.Count} itens");
        
        SaveInventory();
        PrintSaveInfo();
        
        inventoryData.Initialize();
        Debug.Log("🗑️ Inventário limpo");
        
        bool success = LoadInventory();
        
        var finalState = inventoryData.GetCurrentInventoryState();
        Debug.Log($"📊 Estado final: {finalState.Count} itens");
        
        if (currentState.Count == finalState.Count)
        {
            Debug.Log("✅ Teste PASSED!");
        }
        else
        {
            Debug.LogError($"❌ Teste FAILED! {currentState.Count} vs {finalState.Count}");
        }
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
