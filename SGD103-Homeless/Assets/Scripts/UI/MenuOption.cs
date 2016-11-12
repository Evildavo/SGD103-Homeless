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
            if (OptionInfo.OptionTextColourOverride.HasValue)
            {
                OptionText.color = OptionInfo.OptionTextColourOverride.Value;
            }
            else
            {
                OptionText.color = Menu.EnabledOptionColour;
            }
            ValueText.color = Menu.EnabledValueColour;

            if (OptionInfo.BoldText)
            {
                OptionText.fontStyle = FontStyle.Bold;
            }
            else
            {
                OptionText.fontStyle = FontStyle.Normal;
            }
        }
        else
        {
            OptionText.color = Menu.DisabledOptionColour;
            ValueText.color = Menu.DisabledValueColour;
        }
    }
}
