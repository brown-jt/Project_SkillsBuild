using System.Linq;
using UnityEngine;

public class Shelf : InteractableItem
{
    private PlayerInteract playerInteract;
    private string answer;

    private void Start()
    {
        playerInteract = FindFirstObjectByType<PlayerInteract>();
        IsInteractable = false; // Initially, the shelf is not interactable as player won't be holding an item
    }

    private void Update()
    {
        if (playerInteract.IsHoldingItem)
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

        if (answer == correctAnswer) FeedbackNotificationsUI.Instance.AddNotification("Correct! " + data.correctMessage, 10);
        else FeedbackNotificationsUI.Instance.AddNotification("Incorrect! " + data.incorrectMessage, 10);
    }
}
