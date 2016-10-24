using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerCharacter : Character
{
    private float timeAtStartedVomiting;

    public AudioClip VomitSound;
    
    public Color SubmissiveOptionColor = Color.white;
    public Color PridefulOptionColor = Color.white;
    public Color SelfishOptionColor = Color.white;

    public float VomitDurationSeconds;
    [ReadOnly]
    public bool IsVomiting = false;

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
            Main.Menu.Hide();
            callback(ResponseType.SUBMISSIVE);
        };
        Menu.Option.OnSelectedCallback pridefulSelected = () =>
        {
            Main.Menu.Hide();
            callback(ResponseType.PRIDEFUL);
        };
        Menu.Option.OnSelectedCallback selfishSelected = () =>
        {
            Main.Menu.Hide();
            callback(ResponseType.SELFISH);
        };
        Menu.Option.OnSelectedCallback exitSelected = () =>
        {
            Main.Menu.Hide();
            callback(ResponseType.NONE);
        };
        List<Menu.Option> options = new List<Menu.Option>();
        if (submissiveOption != "")
        {
            options.Add(new Menu.Option(submissiveSelected, submissiveOption, 0, true, SubmissiveOptionColor));
        }
        if (pridefulOption != "")
        {
            options.Add(new Menu.Option(pridefulSelected, pridefulOption, 0, true, PridefulOptionColor));
        }
        if (selfishOption != "")
        {
            options.Add(new Menu.Option(selfishSelected, selfishOption, 0, true, SelfishOptionColor));
        }
        options.Add(new Menu.Option(exitSelected, "Exit"));
        Main.Menu.Show(options);
    }

    public void SetWonkyWalkAngle(float angle)
    {
        GetComponent<ThirdPersonCharacter>().SetWonkyWalkAngle(angle * Mathf.Deg2Rad);
    }

    // Makes the player character start to vomit.
    public void Vomit()
    {
        if (!IsVomiting)
        {
            IsVomiting = true;
            timeAtStartedVomiting = Time.time;

            // Restrict movement.
            GetComponent<ThirdPersonCharacter>().VomitStart();

            // Play vomit sound.
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = VomitSound;
            audio.time = 0.0f;
            audio.Play();
        }
    }
	
	new void Update () {
        base.Update();

        // Handle vomiting.
        if (IsVomiting)
        {
            // Stop after time.
            if (Time.time - timeAtStartedVomiting > VomitDurationSeconds)
            {
                IsVomiting = false;
                GetComponent<ThirdPersonCharacter>().VomitEnd();
                GetComponent<AudioSource>().Stop();
            }
        }
	}
}
