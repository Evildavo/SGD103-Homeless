using UnityEngine;
using System.Collections;

public class NewStand : MonoBehaviour
{
    public MessageBox MessageBox;
    public GameTime GameTime;
    public Trigger Trigger;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
    }

    public void OnTrigger()
    {
        const float MESSAGE_TIME = 4.0f;
        MessageBox.ShowForTime("", MESSAGE_TIME, gameObject);
        Trigger.ResetWithCooloff(MESSAGE_TIME + 0.5f);
    }

    void Update()
    {
        if (MessageBox.IsDisplayed() && MessageBox.Source == gameObject)
        {
           
            MessageBox.SetMessage("Today is " + 
                GameTime.DayOfTheWeekAsString(GameTime.DayOfTheWeek));
        }
    }
    
}
