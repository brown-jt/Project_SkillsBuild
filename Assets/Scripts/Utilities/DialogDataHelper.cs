using UnityEngine;
using System.Collections.Generic;

public static class DialogDataHelper
{
    public static DialogData CreateDialogDataFromText(string text)
    {
        DialogData dialog = ScriptableObject.CreateInstance<DialogData>();
        dialog.lines = new List<string>{ text };
        return dialog;
    }
}
