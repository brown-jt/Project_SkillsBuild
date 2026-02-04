[System.Serializable]
public class ObjectiveProgress
{
    public ObjectiveData data;
    public int currentAmount = 0;

    public bool IsComplete => currentAmount >= data.requiredAmount;
}
