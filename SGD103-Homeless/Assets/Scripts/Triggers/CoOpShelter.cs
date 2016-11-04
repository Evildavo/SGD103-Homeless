using UnityEngine;
using System.Collections.Generic;

public class CoOpShelter : MonoBehaviour {
    bool inMainMenu = false;
    bool wasSoupKitchenOpen = false;
    bool wasCounsellingOpen = false;
    bool wasAddictionSupportOpen = false;
    float activeFromHour;
    float activeToHour;
    MenuEnum menu;

    enum MenuEnum
    {
        MAIN,
        CO_OP,
        FOOD,
        OUTDOOR_EQUIPMENT,
        CLOTHING
    }

    public Main Main;
    public Trigger Trigger;
    public EventAtLocation SoupKitchenEvent;
    public EventAtLocation CounsellingEvent;
    public EventAtLocation AddictionSupportEvent;

    public float TimeCostToReadNotice;
    public List<InventoryItem> CoOpMenuPrefabItems;
    [Header("Optionally use these sub-menus if there are too many items")]
    public List<InventoryItem> FoodMenuPrefabItems;
    public List<InventoryItem> OutdoorEquipmentMenuPrefabItems;
    public List<InventoryItem> ClothingMenuPrefabItems;
    public float TimeCostToPurchaseItem;

    void Start ()
    {
        activeFromHour = Trigger.ActiveFromHour;
        activeToHour = Trigger.ActiveToHour;

        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        Trigger.RegisterOnCloseRequested(reset);
    }

    public void OpenMainMenu()
    {
        menu = MenuEnum.MAIN;
        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OpenCoOpShopMenu, "Co-op shop"));
        options.Add(new Menu.Option(ReadNoticeBoard, "Read notice board"));
        /*options.Add(new Menu.Option(RequestEmergencyAccomodation, "Request emergency accommodation"));*/
        if (SoupKitchenEvent && SoupKitchenEvent.IsOpen)
        {
            options.Add(new Menu.Option(AttendSoupKitchen, "Attend soup kitchen"));
        }
        if (CounsellingEvent && CounsellingEvent.IsOpen)
        {
            options.Add(new Menu.Option(AttendCounselling, "Attend counselling"));
        }
        if (AddictionSupportEvent && AddictionSupportEvent.IsOpen)
        {
            options.Add(new Menu.Option(AttendAddictionSupport, "Attend addiction support therapy"));
        }
        options.Add(new Menu.Option(reset, "Exit"));

        Main.Menu.Show(options);
        inMainMenu = true;
    }

    public void OpenCoOpShopMenu()
    {
        menu = MenuEnum.CO_OP;
        Main.MessageBox.ShowNext();
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(CoOpMenuPrefabItems, options);
        if (FoodMenuPrefabItems.Count > 0)
        {
            options.Add(new Menu.Option(OpenFoodMenu, "Buy food"));
        }
        if (OutdoorEquipmentMenuPrefabItems.Count > 0)
        {
            options.Add(new Menu.Option(OpenOutdoorItemMenu, "Buy outdoor equipment"));
        }
        if (ClothingMenuPrefabItems.Count > 0)
        {
            options.Add(new Menu.Option(OpenClothingItemMenu, "Buy clothing"));
        }
        options.Add(new Menu.Option(OpenMainMenu, "Back"));

        Main.Menu.Show(options);
        inMainMenu = false;
    }

    // Adds the given purchase items to the given menu.
    void AddMenuOptions(List<InventoryItem> items, List<Menu.Option> options)
    {
        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem item = items[i];

            // Use the purchase item name if available.
            string name = item.PurchaseItemName;
            if (name == "")
            {
                name = item.ItemName;
            }

            // Add the option. Uses a closure to hold the item selected parameter.
            options.Add(new Menu.Option(
                () => purchaseItemSelected(item),
                name, item.GetItemValue(),
                Main.PlayerState.CanAfford(item.GetItemValue())));
        }
    }
    
    public void OpenFoodMenu()
    {
        menu = MenuEnum.FOOD;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(FoodMenuPrefabItems, options);
        options.Add(new Menu.Option(OpenCoOpShopMenu, "Back"));
        Main.Menu.Show(options);
    }

    public void OpenOutdoorItemMenu()
    {
        menu = MenuEnum.OUTDOOR_EQUIPMENT;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(OutdoorEquipmentMenuPrefabItems, options);
        options.Add(new Menu.Option(OpenCoOpShopMenu, "Back"));
        Main.Menu.Show(options);
    }

    public void OpenClothingItemMenu()
    {
        menu = MenuEnum.CLOTHING;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(ClothingMenuPrefabItems, options);
        options.Add(new Menu.Option(OpenCoOpShopMenu, "Back"));
        Main.Menu.Show(options);
    }

    public void ReadNoticeBoard()
    {
        var GameTime = Main.GameTime;

        // Apply time cost.
        GameTime.SpendTime(TimeCostToReadNotice);

        string message = "";
        if (SoupKitchenEvent)
        {
            message +=
                "Soup Kitchen: " +
                GameTime.DayOfTheWeekAsShortString(SoupKitchenEvent.Day) + " " +
                GameTime.GetTimeAsString(SoupKitchenEvent.FromHour) + " to " +
                GameTime.GetTimeAsString(SoupKitchenEvent.ToHour) + "\n";
        }
        if (CounsellingEvent)
        {
            message += "Counselling: " +
                GameTime.DayOfTheWeekAsShortString(CounsellingEvent.Day) + " " +
                GameTime.GetTimeAsString(CounsellingEvent.FromHour) + " to " +
                GameTime.GetTimeAsString(CounsellingEvent.ToHour) + "\n";
        }
        if (AddictionSupportEvent)
        {
            message += "Addiction Support: " +
                GameTime.DayOfTheWeekAsShortString(AddictionSupportEvent.Day) + " " +
                GameTime.GetTimeAsString(AddictionSupportEvent.FromHour) + " to " +
                GameTime.GetTimeAsString(AddictionSupportEvent.ToHour);
        }
        Main.MessageBox.Show(message, gameObject);
        OpenMainMenu();
    }

    public void RequestEmergencyAccomodation()
    {
        Debug.Log("Emergency accomodation requested");
        OpenMainMenu();
    }

    public void AttendSoupKitchen()
    {
        SoupKitchenEvent.Attend();
        Main.Menu.Hide();
    }

    public void AttendCounselling()
    {
        CounsellingEvent.Attend();
        Main.Menu.Hide();
    }

    public void AttendAddictionSupport()
    {
        AddictionSupportEvent.Attend();
        Main.Menu.Hide();
    }

    public void OnTrigger()
    {
        OpenMainMenu();
    }

    public void OnTriggerUpdate()
    {
        // Notify the user that a service has opened or closed.
        bool serviceOpened = false;
        bool serviceClosed = false;
        if (!wasSoupKitchenOpen && SoupKitchenEvent && SoupKitchenEvent.IsOpen)
        {
            Main.MessageBox.ShowForTime("The soup kitchen has opened for today", 4.0f, gameObject);
            wasSoupKitchenOpen = true;
            serviceOpened = true;
        }
        else if (wasSoupKitchenOpen && SoupKitchenEvent && !SoupKitchenEvent.IsOpen)
        {
            Main.MessageBox.ShowQueued("The soup kitchen has closed for today", 4.0f, gameObject);
            wasSoupKitchenOpen = false;
            serviceClosed = true;
        }
        if (!wasCounsellingOpen && CounsellingEvent && CounsellingEvent.IsOpen)
        {
            Main.MessageBox.ShowForTime("Counselling services have opened for today", 4.0f, gameObject);
            wasCounsellingOpen = true;
            serviceOpened = true;
        }
        else if (wasCounsellingOpen && CounsellingEvent && !CounsellingEvent.IsOpen)
        {
            Main.MessageBox.ShowQueued("Counselling services have closed for today", 4.0f, gameObject);
            wasCounsellingOpen = false;
            serviceClosed = true;
        }
        if (!wasAddictionSupportOpen && AddictionSupportEvent && AddictionSupportEvent.IsOpen)
        {
            Main.MessageBox.ShowForTime("Addiction support therapy has opened for tonight", 4.0f, gameObject);
            wasAddictionSupportOpen = true;
            serviceOpened = true;
        }
        else if (wasAddictionSupportOpen && AddictionSupportEvent && !AddictionSupportEvent.IsOpen)
        {
            Main.MessageBox.ShowQueued("Addiction support therapy has closed for tonight", 4.0f, gameObject);
            wasAddictionSupportOpen = false;
            serviceClosed = true;
        }

        // Update the menu.
        if ((serviceOpened || serviceClosed) && inMainMenu)
        {
            OpenMainMenu();
        }
    }

    public void OnPlayerExit()
    {
        if (SoupKitchenEvent.IsCurrentlyAttending)
        {
            SoupKitchenEvent.Leave();
        }
        if (CounsellingEvent.IsCurrentlyAttending)
        {
            CounsellingEvent.Leave();
        }
        if (AddictionSupportEvent.IsCurrentlyAttending)
        {
            AddictionSupportEvent.Leave();
        }
    }

    void updateMenu()
    {
        switch (menu)
        {
            case MenuEnum.MAIN:
                OpenMainMenu();
                break;
            case MenuEnum.CO_OP:
                OpenCoOpShopMenu();
                break;
            case MenuEnum.FOOD:
                OpenFoodMenu();
                break;
            case MenuEnum.OUTDOOR_EQUIPMENT:
                OpenOutdoorItemMenu();
                break;
            case MenuEnum.CLOTHING:
                OpenClothingItemMenu();
                break;
        }
    }

    void reset()
    {
        Main.Menu.Hide();
        Main.MessageBox.ShowNext();
        Trigger.Reset();
    }

    void purchaseItemSelected(InventoryItem itemSelected)
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Main.PlayerState.Money -= itemSelected.GetItemValue();
            Main.GameTime.SpendTime(TimeCostToPurchaseItem);

            // Add item.
            InventoryItem item = Instantiate(itemSelected);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        updateMenu();
    }

    void Update()
    {
        // Set closing hours differently when an event is on.
        if (SoupKitchenEvent.IsOpen || CounsellingEvent.IsOpen || AddictionSupportEvent.IsOpen)
        {
            Trigger.ActiveFromHour = 0.0f;
            Trigger.ActiveToHour = 24.0f;
        }
        else
        {
            Trigger.ActiveFromHour = activeFromHour;
            Trigger.ActiveToHour = activeToHour;
        }
    }

}
