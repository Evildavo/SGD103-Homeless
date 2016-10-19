using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageBox : MonoBehaviour
{
    private float fromTime;
    private float closeAfterSeconds;

    public Image WarningSymbol;

    public bool IsWarning = false;
    [ReadOnly]
    public GameObject Source;
    
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

    // Opens the message box displaying the given message.
    // Source is used to keep track of who is updating the message box.
    // If the message is a warning it'll show a warning symbol.
    public void Show(string message, GameObject source = null, bool isWarning = false)
    {
        gameObject.SetActive(true);
        GetComponentInChildren<Text>().text = message;
        closeAfterSeconds = 0;
        Source = source;
        IsWarning = isWarning;
        updateWarning();
    }

    // Shows the message box for a number of seconds before closing.
    public void ShowForTime(string message, float seconds, GameObject source = null, bool isWarning = false)
    {
        gameObject.SetActive(true);
        GetComponentInChildren<Text>().text = message;
        Source = source;
        fromTime = Time.time;
        closeAfterSeconds = seconds;
        IsWarning = isWarning;
        updateWarning();
    }

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
    
    void Start () {
        Hide();
    }
	
	void Update () {
        updateWarning();

        // Hide after time.
        if (closeAfterSeconds != 0 && Time.time - fromTime > closeAfterSeconds)
        {
            Hide();
        }
    }
}
