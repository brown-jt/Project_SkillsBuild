using UnityEngine;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    private Queue<string> sentences;

    private void Start()
    {
        sentences = new Queue<string>();
    }
}
