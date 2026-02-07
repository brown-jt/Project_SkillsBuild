public enum ObjectiveType
{
    Find,
    Build,
    Explore,
    TalkTo
}

[System.Serializable]
public class ObjectiveData
{
    public ObjectiveType objectiveType;
    public string taskText;
    public string targetId;
    public int requiredAmount = 1;
}
