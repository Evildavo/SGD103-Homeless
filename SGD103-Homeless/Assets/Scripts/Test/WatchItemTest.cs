using UnityEngine;
using System.Collections;

public class WatchItemTest : InventoryItem
{
    public MessageBox MessageBox;
    public GameTime GameTime;

    public override void OnPrimaryAction()
    {
        MessageBox.Show();
    }

    void Start () {
	
	}
	
	void Update () {

        // Update watch.
        if (MessageBox.IsDisplayed())
        {
            MessageBox.SetMessage("The time is " + GameTime.GetTimeAsString());
        }
    }

}
