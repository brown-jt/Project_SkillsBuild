using System.Linq;
using UnityEngine;

public class Shelf : InteractableItem
{
    private PlayerInteract playerInteract;
    private string answer;
    public QuestionSign[] questionSigns;

    private WarehouseQuizManager quizManager;

    private void Start()
    {
        playerInteract = FindFirstObjectByType<PlayerInteract>();
        quizManager = FindFirstObjectByType<WarehouseQuizManager>();
        IsInteractable = false; // Initially, the shelf is not interactable as player won't be holding an item
    }

    private void Update()
    {
        if (playerInteract.IsHoldingItem && answer != string.Empty)
        {
            IsInteractable = true;
        }
        else
        {
            IsInteractable = false;
        }
    }

    public override void Interact()
    {
        if (!IsInteractable) return;

        // Safeguard to ensure we are holding item before trying to "deposit" on the shelf
        if (playerInteract.IsHoldingItem)
        {
            // Drop item on shelf - for now we just destroy it, but this is where I could update UI to show item on shelf or something like that
            QuestionBox questionBox = (QuestionBox)playerInteract.HeldItem;
            playerInteract.DropHeldItem();
            DestroyImmediate(questionBox.gameObject);

            quizManager.SubmitAnswer(this.answer, questionBox.QuestionData, this);
        }
    }

    public void SetAnswer(string answer)
    {
        this.answer = answer;
    }

    public void DisplayResult(bool correct, QuestionData data)
    {
        foreach (QuestionSign sign in questionSigns)
        {
            if (correct)
            {
                sign.SetText("<color=green><size=125%>Correct!</color></size>\n" + data.correctMessage);
            }
            else
            {
                sign.SetText("<color=red><size=125%>Incorrect!</color></size>\n" + data.incorrectMessage);
            }
        }
    }
}
