using System.Linq;
using UnityEngine;

public class Shelf : InteractableItem
{
    private PlayerInteract playerInteract;
    private string answer;
    public QuestionSign[] questionSigns;

    private void Start()
    {
        playerInteract = FindFirstObjectByType<PlayerInteract>();
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
        if (IsInteractable)
        {
            // Safeguard to ensure we are holding item before trying to "deposit" on the shelf
            if (playerInteract.IsHoldingItem)
            {
                // Drop item on shelf - for now we just destroy it, but this is where I could update UI to show item on shelf or something like that
                QuestionBox questionBox = (QuestionBox)playerInteract.HeldItem;
                playerInteract.DropHeldItem();
                DestroyImmediate(questionBox.gameObject);

                CheckAnswer(questionBox.QuestionData);
            }
        }
    }

    public void SetAnswer(string answer)
    {
        this.answer = answer;
    }

    private void CheckAnswer(QuestionData data)
    {
        // Assuming only one correct answer for simplicity for now
        // TODO: Handle multiple correct answers somehow later
        int correctIndex = data.correctAnswerIndices.FirstOrDefault();
        string correctAnswer = data.answers[correctIndex];

        foreach (QuestionSign sign in questionSigns)
        {
            if (answer == correctAnswer) sign.SetText("<color=green><size=125%>Correct!</color></size>\n" + data.correctMessage);
            else sign.SetText("<color=red><size=125%>Incorrect!</color></size>\n" + data.incorrectMessage);
        }
    }
}
