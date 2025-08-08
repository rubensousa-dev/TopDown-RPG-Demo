using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{

    [SerializeField]
    private bool isStackable;
    public bool IsStackable => isStackable;

    [SerializeField]
    private int maxStackSize = 99;
    public int MaxStackSize => maxStackSize;

    [SerializeField]
    private string itemName;
    public string ItemName => itemName;

    [SerializeField]
    private Sprite icon;
    public Sprite Icon => icon;

    [SerializeField, TextArea(2, 5)]
    private string description;
    public string Description => description;

    [SerializeField]
    private ItemType type;
    public ItemType Type => type;

    [SerializeField]
    private int itemIndex;
    public int ItemIndex => itemIndex;

    public int ID => GetInstanceID();
}

public enum ItemType
{
    Apple,
    Potion,
}
