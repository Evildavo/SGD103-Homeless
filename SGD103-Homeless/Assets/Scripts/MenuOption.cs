using UnityEngine;
using UnityEngine.UI;
using System.Collections;


using UnityEngine.Events;

public class MenuOption : MonoBehaviour {

    public Text OptionText;
    public Text ValueText;
    
    public Menu.Option optionInfo;

    public void OnClick()
    {
        optionInfo.Callback.Invoke(optionInfo.Name, optionInfo.Value);
    }
    
	void Start () {
	
	}
	
	void Update () {
	}
}
