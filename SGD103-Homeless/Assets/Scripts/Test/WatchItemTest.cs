using UnityEngine;
using System.Collections;

public class WatchItemTest : InventoryItem
{
    public MessageBox MessageBox;
    public GameTime GameTime;

    public float CloseAfterSeconds = 1.8f;

    public override void OnPrimaryAction()
    {
        MessageBox.ShowForTime(CloseAfterSeconds, gameObject);
    }
	
	void Update () {
        
        // Update watch.
        if (MessageBox.IsDisplayed() && MessageBox.Source == gameObject)
        {
            MessageBox.SetMessage("The time is " + GameTime.GetTimeAsString());
        }
    }

}
