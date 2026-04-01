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
