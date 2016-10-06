using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    // Option name and optionally a price.
    // The ID is used to reference the option in scripts.
    public struct Option
    {
        public string ID;
        public string Name;
        public int Price;

        public Option(string id, string name, int price = 0)
        {
            ID = id;
            Name = name;
            Price = price;
        }
    };

    // Callback for handling options being selected.
    public delegate void OnOptionSelectedCallback(Option option);

    public Transform MenuOptions;

    // Sets the options menu to display the given list of options.
    // The given callback is called when the given option was pressed.
    public void SetOptions(Option[] options, OnOptionSelectedCallback onOptionSelectedCallback)
    {
        MenuOption[] menuOptions = GetComponentsInChildren<MenuOption>(true);

        // Disable all menu options at first.
        foreach (MenuOption menuOption in menuOptions)
        {
            menuOption.gameObject.SetActive(false);
        }

        // Set and enable options one by one.
        int i = 0;
        foreach (Option option in options)
        {
            menuOptions[i].optionInfo = option;
            menuOptions[i].Callback = onOptionSelectedCallback;
            menuOptions[i].OptionText.text = option.Name;
            if (option.Price != 0)
            {
                menuOptions[i].PriceText.text = "$" + option.Price.ToString("F2");
            }
            else
            {
                menuOptions[i].PriceText.text = "";
            }
            menuOptions[i].gameObject.SetActive(true);
            i++;
        }
    }
    
	void Start ()
    {
        // Disable menu visibility initially.
        MenuOption[] menuOptions = GetComponentsInChildren<MenuOption>();
        foreach (MenuOption menuOption in menuOptions)
        {
            menuOption.gameObject.SetActive(false);
        }
    }
	
	void Update () {
	
	}
}
