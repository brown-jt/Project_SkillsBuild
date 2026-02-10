using UnityEngine;

public class FeedbackNotificationsUI : MonoBehaviour
{
    public static FeedbackNotificationsUI Instance;

    public GameObject notificationPrefab;
    public Transform notificationsRoot;
    private void Awake()
    {
        Instance = this;
    }

    public void AddNotification(string message)
    {
        GameObject notification = Instantiate(notificationPrefab, notificationsRoot);
        NotificationEntryUI entryUI = notification.GetComponent<NotificationEntryUI>();
        entryUI.SetMessage(message);
    }
}
