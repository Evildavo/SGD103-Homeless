using UnityEngine;
using System.Collections;

public class WatchItem : InventoryItem
{
    public float CloseAfterSeconds = 1.8f;

    public bool Tarnished = false;

    public override void OnPrimaryAction()
    {
        Main.MessageBox.ShowForTime("", CloseAfterSeconds, gameObject, false);
    }

    void Update()
    {
        // Update watch.
        var MessageBox = Main.MessageBox;
        if (MessageBox.IsDisplayed() && MessageBox.Source == gameObject)
        {
            MessageBox.SetMessage("The time is " + Main.GameTime.GetTimeAsString());
        }
    }

}
