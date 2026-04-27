using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    public int factoryQuestionIndex = -1;
    public int warehouseQuestionIndex = -1;
    public int museumQuestionIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetQuestionIndexForZone(ZoneId zone)
    {
        return zone switch
        {
            ZoneId.Factory => factoryQuestionIndex,
            ZoneId.Warehouse => warehouseQuestionIndex,
            ZoneId.Museum => museumQuestionIndex,
            _ => -1
        };
    }

    public void SetQuestionIndexForZone(ZoneId zone, int index)
    {
        switch (zone)
        {
            case ZoneId.Factory:
                factoryQuestionIndex = index;
                break;
            case ZoneId.Warehouse:
                warehouseQuestionIndex = index;
                break;
            case ZoneId.Museum:
                museumQuestionIndex = index;
                break;
        }
    }
}