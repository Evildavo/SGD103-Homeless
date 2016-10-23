using UnityEngine;
using System.Collections;

public class NewStand : MonoBehaviour
{
    public Main Main;
    public Trigger Trigger;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
    }

    public void OnTrigger()
    {
        const float MESSAGE_TIME = 4.0f;
        Main.MessageBox.ShowForTime("", MESSAGE_TIME, gameObject);
        Trigger.ResetWithCooloff(MESSAGE_TIME + 0.5f);
    }

    void Update()
    {
        var MessageBox = Main.MessageBox;
        if (MessageBox.IsDisplayed() && MessageBox.Source == gameObject)
        {
           
            MessageBox.SetMessage("Today is " + 
                Main.GameTime.DayOfTheWeekAsString(Main.GameTime.DayOfTheWeek));
        }
    }
    
}
