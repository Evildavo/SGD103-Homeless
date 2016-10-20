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
        MessageBox.ShowForTime("", 4.0f, gameObject);
        Invoke("resetTrigger", 4.0f);
    }

    void resetTrigger()
    {
        Trigger.Reset();
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
