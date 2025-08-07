using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea(2, 5)]
    public string description;
    public ItemType type;
}

public enum ItemType
{
    Apple,
    Potion,
    Weapon,
    Material,
}
