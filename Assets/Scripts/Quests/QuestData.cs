using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questId;
    public string title;

    [TextArea(3, 10)]
    public string description;

    public List<ObjectiveData> objectives;
    public RewardData rewards;

    public QuestData nextQuest;

    public DialogData startDialog;
    public DialogData inProgressDialog;
    public DialogData completedDialog;
}
