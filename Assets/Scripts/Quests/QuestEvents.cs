using System;

public static class QuestEvents
{
    public static Action<string> onFoundItem;
    public static Action<string> onBuiltItem;
    public static Action<string> onAreaExplored;
    public static Action<string> onNPCTalked;
}
