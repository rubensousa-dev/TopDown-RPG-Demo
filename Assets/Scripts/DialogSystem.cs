using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TMP_Text dialogText;


    [Header("Settings")]
    [SerializeField] private float autoHideTime = 3f;
    [SerializeField] private float textSpeed = 0.05f;

    private DialogueSO currentDialogue;
    private int currentLineIndex = 0;
    private bool isDialogActive = false;
    private Coroutine textCoroutine;
    private Coroutine autoHideCoroutine;

    public static DialogSystem Instance { get; private set; }

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
       
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    public void ShowDialog(DialogueSO dialogue)
    {
        if (dialogue == null || dialogue.lines == null || dialogue.lines.Length == 0)
            return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogActive = true;

        dialogPanel.SetActive(true);
        ShowCurrentLine();

        DisablePlayerInput();
    }

    private void ShowCurrentLine()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.lines.Length)
        {
            CloseDialog();
            return;
        }

        if (textCoroutine != null)
            StopCoroutine(textCoroutine);

        textCoroutine = StartCoroutine(TypeText(currentDialogue.lines[currentLineIndex]));

        StartAutoHideTimer();
    }

    private IEnumerator TypeText(string text)
    {
        dialogText.text = "";
        
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void StartAutoHideTimer()
    {
        if (autoHideCoroutine != null)
            StopCoroutine(autoHideCoroutine);

        autoHideCoroutine = StartCoroutine(AutoHideAfterTime());
    }

    private IEnumerator AutoHideAfterTime()
    {
        yield return new WaitForSeconds(autoHideTime);
        
        if (isDialogActive)
        {
            NextLine();
        }
    }

    public void NextLine()
    {
        if (!isDialogActive) return;

        currentLineIndex++;
        ShowCurrentLine();
    }

    public void CloseDialog()
    {
        if (!isDialogActive) return;

        isDialogActive = false;
        dialogPanel.SetActive(false);

        if (textCoroutine != null)
            StopCoroutine(textCoroutine);
        if (autoHideCoroutine != null)
            StopCoroutine(autoHideCoroutine);
    }

    private void DisablePlayerInput()
    {
        InputManager.MoveInput = Vector2.zero;
        InputManager.InteractInput = false;
        InputManager.InventoryAction = false;
    }

    public bool IsDialogActive()
    {
        return isDialogActive;
    }
}
