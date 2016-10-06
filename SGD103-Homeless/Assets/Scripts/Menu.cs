using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    // Option name and optionally a price.
    public struct Option
    {
        public string Name;
        public int Price;

        public Option(string name, int price = 0)
        {
            Name = name;
            Price = price;
        }
    };

    public Transform MenuOptions;

    public void SetOptions(Option[] options)
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
