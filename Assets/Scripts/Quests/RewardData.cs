using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RewardData
{
    public int gold;
    public int experience;

    // @TODO: Replace this with some kind of ItemData later maybe?
    public List<ScriptableObject> items;
}
