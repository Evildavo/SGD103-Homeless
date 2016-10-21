using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MessageBox : MonoBehaviour
{
    private float fromTime;
    private float closeAfterSeconds;
    private Queue<Message> messageQueue = new Queue<Message>();

    // Message that can be queued.
    [System.Serializable]
    public struct Message
    {
        public string Content;
        public float Duration;
        public GameObject Source;
        public bool IsWarning;

        // A duration of zero is considered endless.
        public Message(string content, float duration, GameObject source, bool isWarning)
        {
            Content = content;
            Duration = duration;
            Source = source;
            IsWarning = isWarning;
        }
    }

    public Image WarningSymbol;

    public bool IsWarning = false;
    [ReadOnly]
    public GameObject Source;
    [Header("A gap between queued messages being shown")]
    public float QueuedMessageDelaySeconds = 0.5f;

    void updateWarning()
    {
        if (IsWarning)
        {
            WarningSymbol.enabled = true;
        }
        else
        {
            WarningSymbol.enabled = false;
        }
    }

    void showNext()
    {
        displayMessage(messageQueue.Dequeue());
    }

    // Show the message now.
    void displayMessage(Message message)
    {
        gameObject.SetActive(true);
        GetComponentInChildren<Text>().text = message.Content;
        Source = message.Source;
        fromTime = Time.time;
        closeAfterSeconds = message.Duration;
        IsWarning = message.IsWarning;
        updateWarning();
    }

    // Tells the message box to show the standard "Inventory is full" warning for a couple of seconds.
    // Give a reference to the inventory to open a preview of the inventory.
    public void WarnInventoryFull(Inventory inventory = null)
    {
        ShowForTime("You don't have room in your inventory", 2.0f, null, true);
        if (inventory)
        {
            inventory.ShowPreview();
        }
    }

    // Opens the message box displaying the given message, interrupting any currently shown message.
    // Source is used to keep track of who is updating the message box.
    // If the message is a warning it'll show a warning symbol.
    // @remark Generally used for sustained message boxes (e.g. "Sleeping...").
    public void Show(string message, GameObject source = null, bool isWarning = false)
    {
        displayMessage(new Message(message, 0.0f, source, isWarning));
    }

    // Shows the message box for a number of seconds before closing, interrupting any currently shown message.
    // @remark Generally used to respond to a player's action (e.g. "You feel full" after eating an item),
    // or for an urgent alerts that can't be waited for.
    public void ShowForTime(string message, float seconds, GameObject source = null, bool isWarning = false)
    {
        displayMessage(new Message(message, seconds, source, isWarning));
    }

    // Adds the message to the message queue, to be showed after any currently shown messages are finished.
    // @remark Generally used to notify the player of something (e.g. "The library will close shortly").
    public void ShowQueued(string message, float seconds, GameObject source = null, bool isWarning = false)
    {
        // Add message to queue.
        messageQueue.Enqueue(new Message(message, seconds, source, isWarning));

        // Display the first message if none is displayed.
        if (!IsDisplayed() && messageQueue.Count > 0)
        {
            displayMessage(messageQueue.Dequeue());
        }
    }

    // Closes the current message and shows the next message in the queue if there are any (with a gap in between).
    public void ShowNext()
    {
        Hide();
        if (messageQueue.Count > 0)
        {
            Invoke("showNext", QueuedMessageDelaySeconds);
        }
    }
    
    // Closes the message box.
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsDisplayed()
    {
        return gameObject.activeInHierarchy;
    }

    public void SetMessage(string message)
    {
        GetComponentInChildren<Text>().text = message;
    }

    void Start()
    {
        Hide();
    }

    void Update()
    {
        updateWarning();

        // Hide after time.
        if (closeAfterSeconds != 0 && Time.time - fromTime > closeAfterSeconds)
        {
            Hide();
        }

        // Display the next message in the queue when current message is hidden (with a gap).
        if (!IsDisplayed() && messageQueue.Count > 0)
        {
            Invoke("showNext", QueuedMessageDelaySeconds);
        }
    }

}
