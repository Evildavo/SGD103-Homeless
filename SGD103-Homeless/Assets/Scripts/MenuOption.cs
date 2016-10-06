using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuOption : MonoBehaviour {

    public Text OptionText;
    public Text PriceText;
    
    public Menu.Option optionInfo;
    public Menu.OnOptionSelectedCallback Callback;

    public void OnClick()
    {
        Callback(optionInfo);
    }
    
	void Start () {
	
	}
	
	void Update () {
	}
}
