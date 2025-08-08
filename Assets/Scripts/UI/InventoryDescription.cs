using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDescription : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemDetails;
    [SerializeField] private GameObject buttonsPanels;
    [SerializeField] private Button buttonUse;
    [SerializeField] private Button buttonDrop;
    public System.Action OnUseButtonClicked;
    public System.Action OnDropButtonClicked;

    private void Awake()
    {
        Clear();
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (buttonUse != null)
        {
            buttonUse.onClick.AddListener(() => OnUseButtonClicked?.Invoke());
        }
        
        if (buttonDrop != null)
        {
            buttonDrop.onClick.AddListener(() => OnDropButtonClicked?.Invoke());
        }
    }

    public void Clear()
    {
        buttonsPanels.SetActive(false);
        itemImage.gameObject.SetActive(false);
        itemTitle.text = string.Empty;
        itemDetails.text = string.Empty;
    }

    public void Set(Sprite sprite, string name, string details)
    {
        buttonsPanels.SetActive(true);
        itemImage.sprite = sprite;
        itemImage.gameObject.SetActive(true);
        itemTitle.text = name;
        itemDetails.text = details;
    }
}
