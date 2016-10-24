using UnityEngine;
using System.Collections.Generic;

public class CoOpShelter : MonoBehaviour {

    public Main Main;
    public Trigger Trigger;

	void Start () {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnCloseRequested(reset);
    }

    public void OpenMainMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OpenCoOpShopMenu, "Co-op shop"));
        options.Add(new Menu.Option(ReadNoticeBoard, "Read notice board"));
        options.Add(new Menu.Option(AttendSoupKitchen, "Attend soup kitchen"));
        options.Add(new Menu.Option(AttendCounselling, "Attend counselling"));
        options.Add(new Menu.Option(AttendAddictionSupport, "Attend addiction support therapy"));
        options.Add(new Menu.Option(RequestEmergencyAccomodation, "Request emergency accommodation"));
        options.Add(new Menu.Option(reset, "Exit"));

        Main.Menu.Show(options);
    }

    public void OpenCoOpShopMenu()
    {
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OpenMainMenu, "Exit"));

        Main.Menu.Show(options);
    }

    public void ReadNoticeBoard()
    {
        Debug.Log("Reading notice board");
    }

    public void AttendSoupKitchen()
    {
        Debug.Log("Soup kitchen attended");
    }

    public void AttendCounselling()
    {
        Debug.Log("Counselling attended");
    }

    public void AttendAddictionSupport()
    {
        Debug.Log("Addiction support therapy attended");
    }

    public void RequestEmergencyAccomodation()
    {
        Debug.Log("Emergency accomodation requested");
    }

    public void OnTrigger()
    {
        OpenMainMenu();
    }

    void reset()
    {
        Main.Menu.Hide();
        Trigger.Reset();
    }

}
