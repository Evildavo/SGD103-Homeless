using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Menu : MonoBehaviour
{
    // Option name and optionally a value.
    public struct Option
    {
        // Format for the function to call if the option is selected.
        public delegate void OnSelectedCallback();
        
        public string Name;
        public float Value;
        public OnSelectedCallback Callback;

        // The given callback function is called if the player selects the option.
        // Value is the price. If it's zero it isn't displayed.
        public Option(OnSelectedCallback onSelectedCallback, string name, float value = 0)
        {
            Callback = onSelectedCallback;
            Name = name;
            Value = value;
        }
    };

    public Transform MenuOptions;

    // Sets the options menu to display the given list of options.
    public void Show(List<Option> options)
    {
        gameObject.SetActive(true);
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
            menuOptions[i].OptionText.text = option.Name;
            
            if (option.Value != 0.0f)
            {
                menuOptions[i].ValueText.text = "$" + option.Value.ToString("F2");
            }
            else
            {
                menuOptions[i].ValueText.text = "";
            }
            menuOptions[i].gameObject.SetActive(true);
            i++;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
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
