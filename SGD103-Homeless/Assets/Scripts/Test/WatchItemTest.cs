using UnityEngine;
using System.Collections;

public class WatchItemTest : InventoryItem
{
    private float timeAtActivateSeconds;

    public MessageBox MessageBox;
    public GameTime GameTime;

    public float CloseAfterSeconds = 1.8f;

    public override void OnPrimaryAction()
    {
        MessageBox.Show();
        timeAtActivateSeconds = Time.time;
    }
	
	void Update () {
        
        // Update watch.
        if (MessageBox.IsDisplayed())
        {
            MessageBox.SetMessage("The time is " + GameTime.GetTimeAsString());

            // Close after some time.
            if (Time.time - timeAtActivateSeconds > CloseAfterSeconds)
            {
                MessageBox.Hide();
            }
        }
    }

}
