using UnityEngine;
using System.Collections.Generic;


public class Supermarket : MonoBehaviour
{
    MenuEnum menu;

    enum MenuEnum
    {
        MAIN,
        FOOD,
        OUTDOOR_EQUIPMENT,
        CLOTHING,
        CHANGING_ROOMS,
        LIQUOR,
        MEDICINE
    }
    
    public Main Main;
    public Trigger Trigger;
    public JobTrigger JobTrigger;
    public JobLocation JobLocation;
    public List<InventoryItem> FoodMenuPrefabItems;
    public List<InventoryItem> OutdoorEquipmentMenuPrefabItems;
    public List<InventoryItem> ClothingMenuPrefabItems;
    public List<InventoryItem> LiquorMenuPrefabItems;
    public List<InventoryItem> MedicineMenuPrefabItems;
    public AudioClip Ambience;
    public Sprite Splash;

    public float TimeCostToPurchaseItem;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
        Trigger.RegisterOnCloseRequested(Reset);
    }

    public void OpenMainMenu()
    {
        Main.PlayerState.IsInPrivate = false;
        menu = MenuEnum.MAIN;
        List<Menu.Option> options = new List<Menu.Option>();
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
        if (LiquorMenuPrefabItems.Count > 0)
        {
            options.Add(new Menu.Option(OpenLiquorItemMenu, "Buy liquor"));
        }
        if (MedicineMenuPrefabItems.Count > 0)
        {
            options.Add(new Menu.Option(OpenMedicineItemMenu, "Buy medicine"));
        }
        if (JobLocation.IsJobAvailableToday)
        {
            options.Add(new Menu.Option(ApplyForJob, "Apply for job"));
        }
        if (JobLocation.PlayerHasJobHere)
        {
            // Note that this is a ghost option to inform the player, not to be a useable menu item.
            string message = "Work (" + JobLocation.GetWorkTimeSummaryShort() + ")";
            options.Add(new Menu.Option(null, message, 0, false));
        }
        options.Add(new Menu.Option(Reset, "Exit"));

        Main.Menu.Show(options);
    }

    public void ApplyForJob()
    {
        // Show confirmation box if the player doesn't have a resume.
        ConfirmationBox.OnChoiceMade onChoice = (bool yes) =>
        {
            if (yes)
            {
                JobLocation.ApplyForJob();
            }
        };
        Main.ConfirmationBox.Open(onChoice, "You don't have a resume. Apply anyway?", "Yes", "No");
        OpenMainMenu();
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
        options.Add(new Menu.Option(OnBackSelected, "Back", 0, true, null, true));
        Main.Menu.Show(options);
    }

    public void OpenOutdoorItemMenu()
    {
        menu = MenuEnum.OUTDOOR_EQUIPMENT;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(OutdoorEquipmentMenuPrefabItems, options);
        options.Add(new Menu.Option(OnBackSelected, "Back", 0, true, null, true));
        Main.Menu.Show(options);
    }

    public void OpenClothingItemMenu()
    {
        Main.PlayerState.IsInPrivate = false;
        menu = MenuEnum.CLOTHING;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(ClothingMenuPrefabItems, options);
        options.Add(new Menu.Option(OpenChangingRoomMenu, "Go to changing room"));
        options.Add(new Menu.Option(OnBackSelected, "Back", 0, true, null, true));
        Main.Menu.Show(options);
    }
    
    public void OpenChangingRoomMenu()
    {
        menu = MenuEnum.CHANGING_ROOMS;
        Main.UI.ReturnTo = OpenChangingRoomMenu;
        Main.MessageBox.ShowNext();

        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(OpenClothingItemMenu, "Back", 0, true, null, true));

        // Allow player to use their inventory.
        Main.PlayerState.IsInPrivate = true;
        Main.Inventory.Show();

        Main.Menu.Show(options);
    }

    public void OpenLiquorItemMenu()
    {
        menu = MenuEnum.LIQUOR;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(LiquorMenuPrefabItems, options);
        options.Add(new Menu.Option(OnBackSelected, "Back", 0, true, null, true));
        Main.Menu.Show(options);
    }

    public void OpenMedicineItemMenu()
    {
        menu = MenuEnum.MEDICINE;
        List<Menu.Option> options = new List<Menu.Option>();
        AddMenuOptions(MedicineMenuPrefabItems, options);
        options.Add(new Menu.Option(OnBackSelected, "Back", 0, true, null, true));
        Main.Menu.Show(options);
    }

    public void OnBackSelected()
    {
        OpenMainMenu();
    }

    public void OnTrigger()
    {
        // Show splash screen.
        Main.Splash.Show(Splash);

        // Play ambience audio.
        var audio = GetComponent<AudioSource>();
        audio.clip = Ambience;
        audio.time = 0.0f;
        audio.loop = true;
        audio.Play();

        // Stop street audio.
        if (Main.Ambience)
        {
            Main.Ambience.Pause();
        }

        JobLocation.CheckForJob(true);
        OpenMainMenu();
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

    void updateMenu()
    {
        switch (menu)
        {
            case MenuEnum.MAIN:
                OpenMainMenu();
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
            case MenuEnum.LIQUOR:
                OpenLiquorItemMenu();
                break;
            case MenuEnum.MEDICINE:
                OpenMedicineItemMenu();
                break;
        }
    }

    void Reset()
    {
        // Hide splash screen.
        Main.Splash.Hide();

        // Stop the ambience.
        var audio = GetComponent<AudioSource>();
        audio.Stop();
        audio.clip = null;

        // Resume street audio.
        if (Main.Ambience)
        {
            Main.Ambience.Resume();
        }

        Main.Menu.Hide();
        Main.MessageBox.ShowNext();
        if (Trigger)
        {
            Trigger.Reset(Trigger.IsEnabled);
        }
    }

    public void OnTriggerUpdate()
    {
        // Show warning that job is about to start.
        if (JobLocation.CanWorkNow)
        {
            Reset();
            Main.MessageBox.ShowForTime("Work is about to start.", null, gameObject);
        }
    }

    public void Update()
    {
        // Switch to the job trigger when it's time to start work.
        if (JobLocation.CanWorkNow)
        {
            Trigger.IsEnabled = false;
            JobTrigger.IsEnabled = true;
        }
        else
        {
            Trigger.IsEnabled = true;
            JobTrigger.IsEnabled = false;
        }
    }

}
