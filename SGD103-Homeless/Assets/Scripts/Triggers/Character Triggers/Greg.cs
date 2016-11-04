using UnityEngine;
using System.Collections.Generic;

public class Greg : Character
{
    private bool playerSoldWatch = false;
    
    public Trigger Trigger;
    public AudioClip HelloAudio;
    public AlcoholItem AlcoholPrefab;
    public SonPhotoItem SonPhotoItem;
    public WatchItem WatchPrefab;

    public float SellingToGregValueFactor = 0.65f;
    public float DrugsCost;
    public float AlcoholCost;
    public float WatchBuyBackCost;
    public float TimeCostForBuyingItem;
    public float TimeCostForSellingItem;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        Trigger.RegisterOnCloseRequested(Reset);
    }

    new void Update()
    {
        base.Update();
    }

    public void OnTrigger()
    {
        Speak("Hi, my name's Greg. Would you like to buy something from me?", HelloAudio);
        Main.PlayerCharacter.ShowStandardDialogueMenu(
            "Can you help me?",
            "I can manage",
            "Give me stuff now",
            onResponseChosen);

        //Speak("Hi, my name's Greg. Would you like to buy something?", HelloAudio, showBuyMenu);
        //AddCaptionChangeCue(3.0f, "Want some drugs?");
    }

    void onResponseChosen(PlayerCharacter.ResponseType response)
    {
        if (response == PlayerCharacter.ResponseType.SUBMISSIVE)
        {
            Speak("Sure can. What are you after?");
            AddCaptionChangeCue(2.0f, "Drugs perhaps?");
            showBuySellMenu();
        }
        else if (response == PlayerCharacter.ResponseType.PRIDEFUL)
        {
            Speak("Have it your way.");
            Reset();
        }
        else if (response == PlayerCharacter.ResponseType.SELFISH)
        {
            Speak("Hey, there's no need for that sort of attitude.");
            AddCaptionChangeCue(2.0f, "You look like you could use some drugs.");
            showBuySellMenu();
        }
        else
        {
            Reset();
        }
    }

    public void OnPlayerExit()
    {
        if (IsSpeaking)
        {
            Speak("Hey, where are you going?");
        }
    }

    public void Reset()
    {
        Main.Menu.Hide();
        Trigger.Reset();
        Main.Inventory.ExitSellMode();
    }

    void showBuySellMenu()
    {
        Main.Inventory.EnterSellMode(onPlayerSellingItem);

        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(onBuyDrugsSelected, "Drugs", DrugsCost, Main.PlayerState.CanAfford(DrugsCost)));
        options.Add(new Menu.Option(onBuyAlcoholSelected, "Cheap Alcohol", AlcoholCost, Main.PlayerState.CanAfford(AlcoholCost)));
        if (playerSoldWatch)
        {
            options.Add(new Menu.Option(onBuyWatchSelected, "Buy Watch", WatchBuyBackCost, Main.PlayerState.CanAfford(WatchBuyBackCost)));
        }
        options.Add(new Menu.Option(Reset, "Exit"));

        Main.Menu.Show(options);
    }

    void onBuyDrugsSelected()
    {
        Main.MessageBox.ShowForTime("I just can't do that", null);
        //PlayerCharacter.Speak("I just can't do that");
        SonPhotoItem.ShowPhoto();
        showBuySellMenu();
    }

    void onBuyAlcoholSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Speak("Sure thing.");

            // Remove money.
            Main.PlayerState.Money -= AlcoholCost;
            
            // Apply time cost.
            Main.GameTime.SpendTime(TimeCostForBuyingItem);

            // Add item.
            AlcoholItem item = Instantiate(AlcoholPrefab);
            item.Main = Main;
            Main.Inventory.AddItem(item);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        showBuySellMenu();
    }

    void onBuyWatchSelected()
    {
        if (!Main.Inventory.IsInventoryFull)
        {
            Speak("Just couldn't part with it eh?");
            AddCaptionChangeCue(2.0f, "In near-original condition too");

            // Remove money.
            Main.PlayerState.Money -= WatchBuyBackCost;

            // Apply time cost.
            Main.GameTime.SpendTime(TimeCostForBuyingItem);

            // Add item.
            WatchItem item = Instantiate(WatchPrefab);
            item.Main = Main;
            item.ItemName = "Watch (tarnished)";
            item.ItemValue = item.GetItemValue() * 0.75f;
            item.Tarnished = true;
            Main.Inventory.AddItem(item);

            // Put watch back on HUD.
            var WatchHudButton = Main.HudButtons.WatchHudButton;
            WatchHudButton.Watch = item;
            WatchHudButton.gameObject.SetActive(true);
        }
        else
        {
            Main.MessageBox.WarnInventoryFull(Main.Inventory);
        }
        showBuySellMenu();
    }

    void onPlayerSellingItem(InventoryItem item)
    {
        float gregOffer = Mathf.Floor(item.GetItemValue() * SellingToGregValueFactor);

        ConfirmationBox.OnChoiceMade onChoice = (bool yes) =>
        {
            if (yes)
            {
                // Check if the item being sold is the watch.
                if (item.ItemName == "Watch" || item.ItemName == "Watch (tarnished)")
                {
                    playerSoldWatch = true;
                }

                // Player sold the item to Greg.
                Main.PlayerState.Money += gregOffer;

                // Apply time cost.
                Main.GameTime.SpendTime(TimeCostForSellingItem);

                Main.Inventory.RemoveItem(item);
                showBuySellMenu();
            }
        };
        Main.ConfirmationBox.Open(onChoice, 
            "I'll give you $" + gregOffer.ToString("f2") + " for it. That ok?", "Yes", "No");
    }

}
