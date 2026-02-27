using System;

public static class QuestEvents
{
    public static Action<string> onFoundItem;
    public static Action<string> onBuiltItem;
    public static Action<string> onAreaExplored;
    public static Action<string> onNPCTalked;
    public static Action<string> onQuizPassed;

    public static void ItemFound(string itemId)
    {
        onFoundItem?.Invoke(itemId);
    }

    public static void ItemBuilt(string itemId)
    {
        onBuiltItem?.Invoke(itemId);
    }

    public static void AreaExplored(string areaId)
    {
        onAreaExplored?.Invoke(areaId);
    }

    public static void NPCTalked(string npcId)
    {
        onNPCTalked?.Invoke(npcId);
    }
    public static void QuizPassed(string quizId)
    {
        onQuizPassed?.Invoke(quizId);
    }

}
