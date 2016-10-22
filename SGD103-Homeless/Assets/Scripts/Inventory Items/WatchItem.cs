using UnityEngine;
using System.Collections;

public class WatchItem : InventoryItem
{
    public MessageBox MessageBox;
    public GameTime GameTime;

    public float CloseAfterSeconds = 1.8f;

    public bool Tarnished = false;

    public override void OnPrimaryAction()
    {
        MessageBox.ShowForTime("", CloseAfterSeconds, gameObject, false);
    }

    void Update()
    {
        // Update watch.
        if (MessageBox.IsDisplayed() && MessageBox.Source == gameObject)
        {
            MessageBox.SetMessage("The time is " + GameTime.GetTimeAsString());
        }
    }

}
