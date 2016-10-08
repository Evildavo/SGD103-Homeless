using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Menu : MonoBehaviour
{
    // Option name and optionally a value.
    [System.Serializable]
    public struct Option
    {
        // The parameters are name and value.
        [System.Serializable]
        public class OnSelectedEvent : UnityEvent<string, int> { }
              
        public string Name;
        public int Value;
        public OnSelectedEvent Callback;

        public Option(OnSelectedEvent callback, string name, int value = 0)
        {
            Callback = callback;
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
            if (option.Value != 0)
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
