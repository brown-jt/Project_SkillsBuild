using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Dialog Data")]
public class DialogData : ScriptableObject
{
    [TextArea] 
    public List<string> lines;
}
