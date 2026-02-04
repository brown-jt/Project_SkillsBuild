using System.Collections.Generic;

public class QuestInstance
{
    public QuestData questData;
    public List<ObjectiveProgress> objectivesProgress;

    public bool IsObjectivesComplete => objectivesProgress.TrueForAll(obj => obj.IsComplete);
    public bool IsTurnedIn = false;

    public QuestInstance(QuestData data)
    {
        this.questData = data;
        objectivesProgress = new List<ObjectiveProgress>();

        foreach (var objectiveData in data.objectives)
        {
            objectivesProgress.Add(new ObjectiveProgress { data = objectiveData });
        }
    }
}
