using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

    [SerializeField] private ProgressionBar GSWAI;
    [SerializeField] private ProgressionBar GSWD;
    [SerializeField] private ProgressionBar GSWGA;

    private void Awake()
    {
        // Ensure singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        QuestManager.Instance.DatabaseQuestsLoaded += DatabaseQuestsLoaded;
    }

    private void OnDisable()
    {
        QuestManager.Instance.DatabaseQuestsLoaded -= DatabaseQuestsLoaded;
    }

    private void DatabaseQuestsLoaded()
    {
        // Initialise bars before adding experience to ensure they are ready to receive updates
        GSWAI.InitialiseBar();
        GSWD.InitialiseBar();
        GSWGA.InitialiseBar();

        foreach (QuestInstance quest in QuestManager.Instance.completedQuests)
        {
            QuestData questData = quest.questData;
            AddExperience(questData.zoneId, questData.rewards.experience);
        }
    }

    public void AddExperience(ZoneId zoneId, int experience)
    {
        switch (zoneId)
        {
            case ZoneId.Factory:
                GSWAI.AddExperience(experience);
                break;
            case ZoneId.Warehouse:
                GSWD.AddExperience(experience);
                break;
            case ZoneId.Museum:
                GSWGA.AddExperience(experience);
                break;
        }
    }
}
