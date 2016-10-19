using UnityEngine;
using UnityEngine.UI;
using System.Collections;


using UnityEngine.Events;

public class MenuOption : MonoBehaviour {

    public Menu Menu;
    public Text OptionText;
    public Text ValueText;
    
    public Menu.Option OptionInfo;

    public void OnClick()
    {
        if (OptionInfo.Enabled)
        {
            OptionInfo.Callback();
        }
    }
    
	void Update ()
    {
        if (OptionInfo.Enabled)
        {
            OptionText.color = Menu.EnabledOptionColour;
            ValueText.color = Menu.EnabledValueColour;
        }
        else
        {
            OptionText.color = Menu.DisabledOptionColour;
            ValueText.color = Menu.DisabledValueColour;
        }
    }
}
