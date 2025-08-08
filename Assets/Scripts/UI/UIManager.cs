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
    
    // Singleton para acesso global
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

    // Método público para acessar o inventário
    public InventorySO GetInventory()
    {
        return inventoryData;
    }

    // Método público para adicionar item ao inventário
    public bool AddItemToInventory(ItemSO item, int quantity)
    {
        if (inventoryData == null) return false;
        
        int remaining = inventoryData.AddItem(item, quantity);
        bool success = remaining == 0;
        
        if (success)
        {
            // Mostrar informação sobre o item coletado
            ShowItemCollectedNotification(item, quantity);
        }
        else
        {
            // Mostrar notificação de inventário cheio
            ShowInventoryFullNotification();
        }
        
        return success;
    }

    private void ShowItemCollectedNotification(ItemSO item, int quantity)
    {
        string message = quantity > 1 ? $"+{quantity} {item.ItemName}s" : $"+1 {item.ItemName}";
        Debug.Log($"✅ {message} coletado!");
        
        // Mostrar quantidade total no inventário
        int totalCount = inventoryData.GetItemCount(item);
        Debug.Log($"Total de {item.ItemName}s no inventário: {totalCount}");
    }

    private void ShowInventoryFullNotification()
    {
        Debug.Log("⚠️ INVENTÁRIO CHEIO! ⚠️");
        // Aqui você pode adicionar uma UI de notificação se quiser
        // Por exemplo, mostrar um texto na tela por alguns segundos
    }

    private void PrepareInventoryData()
    {
        // Tentar carregar dados salvos primeiro
        InventorySaveSystem saveSystem = FindFirstObjectByType<InventorySaveSystem>();
        bool loadedFromSave = false;
        
        if (saveSystem != null)
        {
            loadedFromSave = saveSystem.LoadInventory();
        }
        else
        {
            Debug.LogWarning("⚠️ InventorySaveSystem não encontrado na cena! Criar um GameObject com o script InventorySaveSystem.");
        }
        
        // Se não conseguiu carregar, inicializar com itens padrão
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
        // Salvar o índice do item atualmente selecionado antes do reset
        int? currentSelectedIndex = inventoryUI.LastSelectedItemIndex;
        
        inventoryUI.ResetAllItems();
        foreach (var item in state)
        {
            inventoryUI.UpdateData(item.Key, item.Value.item.Icon, item.Value.quantity);
        }
        
        // Restaurar a seleção se havia um item selecionado
        if (currentSelectedIndex.HasValue && currentSelectedIndex.Value < state.Count)
        {
            // Verificar se o item ainda existe na posição selecionada
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
        
        // Usar o item
        UseItemFromInventory(itemData.item);
        
        // Remover 1 do inventário
        inventoryData.RemoveItem(item, 1);
    }
    
    private void HandleUseItemRequest(int item)
    {
        InventoryItem itemData = inventoryData.GetItemAt(item);
        if (itemData.IsEmpty) return;
        
        // Usar o item
        UseItemFromInventory(itemData.item);
        
        // Remover 1 do inventário
        inventoryData.RemoveItem(item, 1);
        
        // Fechar o inventário
        inventoryUI.Hide();
        
        Debug.Log($"✅ Item '{itemData.item.ItemName}' usado e inventário fechado!");
    }
    
    private void HandleDropItemRequest(int item)
    {
        InventoryItem itemData = inventoryData.GetItemAt(item);
        if (itemData.IsEmpty) return;
        
        // Remover o item completamente do inventário
        inventoryData.RemoveItem(item, itemData.quantity);
        
        Debug.Log($"🗑️ Item '{itemData.item.ItemName}' (x{itemData.quantity}) eliminado do inventário!");
    }
    
    private void UseItemFromInventory(ItemSO item)
    {
        // Encontrar o PlayerModification na cena
        PlayerModification playerMod = FindFirstObjectByType<PlayerModification>();
        
        if (playerMod != null)
        {
            playerMod.UseItem(item);
        }
        else
        {
            Debug.LogWarning("PlayerModification não encontrado na cena!");
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

        // Teste do sistema de stacking - pressione T para adicionar maçãs
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestStacking();
        }
        
        // Teste do sistema de uso de itens - pressione U para usar primeiro item
        if (Input.GetKeyDown(KeyCode.U))
        {
            TestUseItem();
        }
        
        // Cancelar efeitos ativos - pressione C
        if (Input.GetKeyDown(KeyCode.C))
        {
            CancelAllEffects();
        }
        
        // Teste do sistema de save/load - pressione S para salvar manualmente
        if (Input.GetKeyDown(KeyCode.S))
        {
            TestSaveInventory();
        }
        
        // Teste do sistema de save/load - pressione L para carregar manualmente
        if (Input.GetKeyDown(KeyCode.L))
        {
            TestLoadInventory();
        }
        
        // Teste do sistema de save/load - pressione X para limpar dados salvos
        if (Input.GetKeyDown(KeyCode.X))
        {
            TestClearSaveData();
        }
        
        // Teste do sistema de save/load - pressione P para mostrar informações do save
        if (Input.GetKeyDown(KeyCode.P))
        {
            TestPrintSaveInfo();
        }
        
        // Teste do sistema de save/load - pressione R para testar save/load
        if (Input.GetKeyDown(KeyCode.R))
        {
            TestSaveLoadCycle();
        }
        
        // Teste do sistema de save/load - pressione I para testar busca de ItemSOs
        if (Input.GetKeyDown(KeyCode.I))
        {
            TestFindItemSOs();
        }
        
        // Teste do sistema de botões - pressione B para testar botões
        if (Input.GetKeyDown(KeyCode.B))
        {
            TestButtonSystem();
        }
        
        // Teste do problema de save/load - pressione D para diagnosticar
        if (Input.GetKeyDown(KeyCode.D))
        {
            TestSaveLoadProblem();
        }
        
        // Teste do novo sistema de lista - pressione N para testar
        if (Input.GetKeyDown(KeyCode.N))
        {
            TestNewListSystem();
        }
    }

    private void TestStacking()
    {
        // Encontrar o ItemSO da maçã
        ItemSO appleItem = null;
        foreach (var item in initialItems)
        {
            if (item.item != null && item.item.ItemName == "Apple")
            {
                appleItem = item.item;
                break;
            }
        }

        if (appleItem != null)
        {
            // Adicionar 5 maçãs para testar o stacking
            int remaining = inventoryData.AddItem(appleItem, 5);
            Debug.Log($"Adicionadas 5 maçãs. Restantes que não couberam: {remaining}");
            
            // Mostrar quantidade total de maçãs no inventário
            int totalApples = inventoryData.GetItemCount(appleItem);
            Debug.Log($"Total de maçãs no inventário: {totalApples}");
        }
    }

    private void TestUseItem()
    {
        // Encontrar o primeiro item não vazio no inventário
        var inventoryState = inventoryData.GetCurrentInventoryState();
        if (inventoryState.Count > 0)
        {
            // Pegar o primeiro item do dicionário
            var firstItem = inventoryState.FirstOrDefault();
            if (firstItem.Value.item != null)
            {
                UseItemFromInventory(firstItem.Value.item);
                inventoryData.RemoveItem(firstItem.Key, 1);
                Debug.Log($"Item '{firstItem.Value.item.ItemName}' usado!");
            }
        }
        else
        {
            Debug.Log("Inventário vazio!");
        }
    }

    private void CancelAllEffects()
    {
        PlayerModification playerMod = FindFirstObjectByType<PlayerModification>();
        if (playerMod != null)
        {
            playerMod.CancelAllEffects();
        }
    }

    private void TestSaveInventory()
    {
        InventorySaveSystem saveSystem = FindFirstObjectByType<InventorySaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.SaveInventory();
            
            // Mostrar informações sobre o save
            var inventoryState = inventoryData.GetCurrentInventoryState();
            Debug.Log($"💾 Inventário salvo com {inventoryState.Count} itens!");
            
            foreach (var item in inventoryState)
            {
                Debug.Log($"  - Slot {item.Key}: {item.Value.quantity}x {item.Value.item.ItemName}");
            }
        }
    }

    private void TestLoadInventory()
    {
        InventorySaveSystem saveSystem = FindFirstObjectByType<InventorySaveSystem>();
        if (saveSystem != null)
        {
            bool success = saveSystem.LoadInventory();
            if (success)
            {
                Debug.Log("✅ Inventário recarregado com sucesso!");
            }
            else
            {
                Debug.Log("❌ Falha ao recarregar inventário!");
            }
        }
    }

    private void TestClearSaveData()
    {
        InventorySaveSystem saveSystem = FindFirstObjectByType<InventorySaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.ClearSaveData();
        }
    }

    private void TestPrintSaveInfo()
    {
        InventorySaveSystem saveSystem = FindFirstObjectByType<InventorySaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.PrintSaveInfo();
        }
    }

    private void TestSaveLoadCycle()
    {
        Debug.Log("🧪 Testando ciclo completo de Save/Load...");
        
        // 1. Salvar inventário atual
        InventorySaveSystem saveSystem = FindFirstObjectByType<InventorySaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.SaveInventory();
            
            // 2. Mostrar estado atual
            var currentState = inventoryData.GetCurrentInventoryState();
            Debug.Log($"📊 Estado atual: {currentState.Count} itens");
            
            // 3. Limpar inventário
            inventoryData.Initialize();
            Debug.Log("🗑️ Inventário limpo");
            
            // 4. Carregar dados salvos
            bool success = saveSystem.LoadInventory();
            if (success)
            {
                var loadedState = inventoryData.GetCurrentInventoryState();
                Debug.Log($"✅ Carregado: {loadedState.Count} itens");
                
                // 5. Comparar estados
                if (currentState.Count == loadedState.Count)
                {
                    Debug.Log("🎉 Teste PASSED! Save/Load funcionando corretamente!");
                }
                else
                {
                    Debug.LogError("❌ Teste FAILED! Estados diferentes!");
                }
            }
            else
            {
                Debug.LogError("❌ Teste FAILED! Falha ao carregar!");
            }
        }
        else
        {
            Debug.LogError("❌ InventorySaveSystem não encontrado!");
        }
    }

    private void TestFindItemSOs()
    {
        InventorySaveSystem saveSystem = FindFirstObjectByType<InventorySaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.TestFindItemSOs();
        }
    }

    private void TestButtonSystem()
    {
        Debug.Log("🧪 Testando sistema de botões...");
        
        // Simular clique no botão Use do primeiro item
        var inventoryState = inventoryData.GetCurrentInventoryState();
        if (inventoryState.Count > 0)
        {
            var firstItem = inventoryState.FirstOrDefault();
            Debug.Log($"🔘 Simulando clique no botão Use para item '{firstItem.Value.item.ItemName}' no slot {firstItem.Key}");
            HandleUseItemRequest(firstItem.Key);
        }
        else
        {
            Debug.Log("❌ Inventário vazio! Adicione itens primeiro (tecla T).");
        }
    }

    private void TestSaveLoadProblem()
    {
        InventorySaveSystem saveSystem = FindFirstObjectByType<InventorySaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.TestSaveLoadProblem();
        }
        else
        {
            Debug.LogError("❌ InventorySaveSystem não encontrado!");
        }
    }
    
    private void TestNewListSystem()
    {
        Debug.Log("🧪 Testando novo sistema de lista...");
        
        // Adicionar vários itens em slots diferentes
        var inventoryState = inventoryData.GetCurrentInventoryState();
        Debug.Log($"📊 Estado atual: {inventoryState.Count} itens");
        
        foreach (var item in inventoryState)
        {
            Debug.Log($"  - Slot {item.Key}: {item.Value.quantity}x {item.Value.item.ItemName}");
        }
        
        // Testar save/load
        InventorySaveSystem saveSystem = FindFirstObjectByType<InventorySaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.TestSaveLoadProblem();
        }
    }
}
