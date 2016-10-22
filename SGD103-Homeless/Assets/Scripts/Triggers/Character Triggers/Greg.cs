using UnityEngine;
using System.Collections.Generic;

public class Greg : Character
{
    public Trigger Trigger;
    public PlayerCharacter PlayerCharacter;
    public AudioClip HelloAudio;
    public Menu Menu;
    public ConfirmationBox ConfirmationBox;
    public PlayerState PlayerState;
    public Inventory Inventory;
    public AlcoholItem AlcoholPrefab;
    public SonPhotoItem SonPhotoItem;

    public float SellingToGregValueFactor = 0.65f;
    public float DrugsCost;
    public float AlcoholCost;

    new void Start()
    {
        base.Start();
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        Trigger.RegisterOnTriggerUpdateListener(OnTriggerUpdate);
    }

    new void Update()
    {
        base.Update();
    }

    public void OnTrigger()
    {
        Speak("Hi, my name's Greg. Would you like to buy something from me?", HelloAudio);
        PlayerCharacter.ShowStandardDialogueMenu(
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
            reset();
        }
        else if (response == PlayerCharacter.ResponseType.SELFISH)
        {
            Speak("Hey, there's no need for that sort of attitude.");
            AddCaptionChangeCue(2.0f, "You look like you could use some drugs.");
            showBuySellMenu();
        }
        else
        {
            reset();
        }
    }

    public void OnPlayerExit()
    {
        if (IsSpeaking)
        {
            Speak("Hey, where are you going?");
        }
        reset();
    }

    public void OnTriggerUpdate()
    {
        // Leave menu on E key.
        if (Menu.IsDisplayed() && !IsSpeaking && Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            reset();
        }
    }

    void showBuySellMenu()
    {
        Inventory.EnterSellMode(onPlayerSellingItem);

        List<Menu.Option> options = new List<Menu.Option>();
        options.Add(new Menu.Option(onBuyDrugsSelected, "Drugs", DrugsCost, PlayerState.CanAfford(DrugsCost)));
        options.Add(new Menu.Option(onBuyAlcoholSelected, "Cheap Alcohol", AlcoholCost, PlayerState.CanAfford(AlcoholCost)));
        options.Add(new Menu.Option(onExitSelected, "Exit"));

        Menu.Show(options);
    }

    void onBuyDrugsSelected()
    {
        MessageBox.ShowForTime("I just can't do that", 2.0f);
        //PlayerCharacter.Speak("I just can't do that");
        SonPhotoItem.ShowPhoto();
        showBuySellMenu();
    }

    void onBuyAlcoholSelected()
    {
        if (!Inventory.IsInventoryFull)
        {
            Speak("Sure thing.");

            // Remove money.
            PlayerState.Money -= AlcoholCost;

            // Add item.
            AlcoholItem item = Instantiate(AlcoholPrefab);
            item.InventoryItemDescription = Inventory.ItemDescription;
            item.MessageBox = MessageBox;
            Inventory.AddItem(item);
        }
        else
        {
            MessageBox.WarnInventoryFull(Inventory);
        }
        showBuySellMenu();
    }

    void onPlayerSellingItem(InventoryItem item)
    {
        float gregOffer = Mathf.Floor(item.ItemValue * SellingToGregValueFactor);

        ConfirmationBox.OnChoiceMade onChoice = (bool yes) =>
        {
            if (yes)
            {
                // Player sold the item to Greg.
                PlayerState.Money += gregOffer;

                Inventory.RemoveItem(item);
                showBuySellMenu();
            }
        };
        ConfirmationBox.Open(onChoice, 
            "I'll give you $" + gregOffer.ToString("f2") + " for it. That ok?", "Yes", "No");
    }

    void onExitSelected()
    {
        reset();
    }

    void reset()
    {
        Menu.Hide();
        Trigger.Reset();
        Inventory.ExitSellMode();
    }

}
