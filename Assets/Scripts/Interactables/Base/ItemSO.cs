using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    [field: SerializeField]
    public bool IsStackable { get; set; }
    public int ID => GetInstanceID();
    [field: SerializeField]
    public string itemName { get; set; }
    [field: SerializeField]
    public Sprite icon { get; set; }
    [TextArea(2, 5)]
    public string description { get; set; }
    [field: SerializeField]
    public ItemType type { get; set; }

}

public enum ItemType
{
    Apple,
    Potion,
}
