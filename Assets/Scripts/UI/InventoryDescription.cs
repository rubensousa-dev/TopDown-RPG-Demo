using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDescription : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemDetails;

    private void Awake()
    {
        Clear();
    }

    public void Clear()
    {
        itemImage.gameObject.SetActive(false);
        itemTitle.text = string.Empty;
        itemDetails.text = string.Empty;
    }

    public void Set(Sprite sprite, string name, string details)
    {
        itemImage.sprite = sprite;
        itemImage.gameObject.SetActive(true);

        itemTitle.text = name;
        itemDetails.text = details;
    }
}
