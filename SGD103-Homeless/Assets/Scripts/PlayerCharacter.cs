using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : Character
{

    public Menu Menu;

    // Standard types of response.
    public enum ResponseType
    {
        NONE,
        SUBMISSIVE,
        PRIDEFUL,
        SELFISH
    }

    // Callback for when a response is chosen.
    public delegate void OnResponse(ResponseType responseType); 

    // Shows the standard dialogue response menu.
    // Leave an option blank to not include it.
    // The given callback is run when a choice is made.
    public void ShowStandardDialogueMenu(
        string submissiveOption,
        string pridefulOption,
        string selfishOption,
        OnResponse callback)
    {
        Menu.Option.OnSelectedCallback submissiveSelected = () =>
        {
            Menu.Hide();
            callback(ResponseType.SUBMISSIVE);
        };
        Menu.Option.OnSelectedCallback pridefulSelected = () =>
        {
            Menu.Hide();
            callback(ResponseType.PRIDEFUL);
        };
        Menu.Option.OnSelectedCallback selfishSelected = () =>
        {
            Menu.Hide();
            callback(ResponseType.SELFISH);
        };
        Menu.Option.OnSelectedCallback exitSelected = () =>
        {
            Menu.Hide();
            callback(ResponseType.NONE);
        };
        List<Menu.Option> options = new List<Menu.Option>();
        if (submissiveOption != "")
        {
            options.Add(new Menu.Option(submissiveSelected, submissiveOption));
        }
        if (pridefulOption != "")
        {
            options.Add(new Menu.Option(pridefulSelected, pridefulOption));
        }
        if (selfishOption != "")
        {
            options.Add(new Menu.Option(selfishSelected, selfishOption));
        }
        options.Add(new Menu.Option(exitSelected, "Exit"));
        Menu.Show(options);
    }
    
	new void Start () {
        base.Start();
	}
	
	new void Update () {
        base.Update();
	}
}
