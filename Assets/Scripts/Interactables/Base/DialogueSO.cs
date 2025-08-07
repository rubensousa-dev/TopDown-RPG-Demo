using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Interaction/Dialogue")]
public class DialogueSO : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] lines;
}
