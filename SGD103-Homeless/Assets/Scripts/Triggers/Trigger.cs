using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Trigger : MonoBehaviour
{
    private TriggerListener onTrigger;
    private TriggerListener onTriggerUpdate;
    private TriggerListener onPlayerEnter;
    private TriggerListener onPlayerExit;
    private TriggerListener onTriggerClose;

    // The format for trigger listener functions.
    public delegate void TriggerListener();

    public PlayerCharacter PlayerCharacter;
    public PlayerState PlayerState;
    public GameTime GameTime;
    public Text TriggerNameText;
    public Text InteractHintText;

    public bool IsEnabled = true;
    public string TriggerName;
    public string InteractHintMessage;
    [Header("Leave blank to not show an interact message")]
    public string OutOfHoursMessage;
    [Header("Note: Also supports wrapping over (e.g. 11pm to 2am)")]
    [Range(0.0f, 24.0f)]
    public float ActiveFromHour = 0.0f;
    [Range(0.0f, 24.0f)]
    public float ActiveToHour = 24.0f;
    public Color TextColour = Color.black;

    [ReadOnly]
    public bool IsPlayerInsideTriggerZone = false;
    [ReadOnly]
    public bool IsActivated = false;
    [ReadOnly]
    public bool IsInActiveHour = false;

    public bool ExitPressed()
    {
        return Input.GetButtonDown("Secondary") ||
               Input.GetKeyDown("e") ||
               Input.GetKeyDown("enter") ||
               Input.GetKeyDown("return");
    }

    // Register the function to call when the player activates the trigger.
    public void RegisterOnTriggerListener(TriggerListener function)
    {
        onTrigger = function;
    }

    // Register the function to call while the trigger is activated.
    public void RegisterOnTriggerUpdateListener(TriggerListener function)
    {
        onTriggerUpdate = function;
    }

    // Register the function to call when the player enters the trigger zone.
    public void RegisterOnPlayerEnterListener(TriggerListener function)
    {
        onPlayerEnter = function;
    }

    // Register the function to call when the player exists the trigger zone.
    public void RegisterOnPlayerExitListener(TriggerListener function)
    {
        onPlayerExit = function;
    }

    // Register the function to call when the player attempts to close or exit the trigger.
    // Call Reset() on the trigger to actually exit.
    public void RegisterOnCloseRequested(TriggerListener function)
    {
        onTriggerClose = function;
    }

    // Requests that the trigger be closed (may be reset).
    public void Close()
    {
        if (onTriggerClose != null)
        {
            onTriggerClose();
        }
        else
        {
            Reset();
        }
    }

    // Resets the trigger after being triggered. 
    // If enabled is false the player won't be able to reactivate the trigger.
    public void Reset(bool enabled = true)
    {
        IsEnabled = enabled;
        IsActivated = false;
        PlayerState.CurrentTrigger = null;
    }

    // Resets the trigger back to enabled after a cooloff time.
    public void ResetWithCooloff(float seconds = 1.0f)
    {
        Invoke("reenableTrigger", seconds);
        IsActivated = false;
        PlayerState.CurrentTrigger = null;
    }

    void reenableTrigger()
    {
        IsEnabled = true;
    }

    public void ShowInteractionText()
    {
        if (TriggerNameText && InteractHintText)
        {
            if (IsInActiveHour || (!IsInActiveHour && OutOfHoursMessage != ""))
            {
                TriggerNameText.GetComponent<Text>().enabled = true;
                InteractHintText.GetComponent<Text>().enabled = true;
                TriggerNameText.GetComponent<Text>().color = TextColour;
                InteractHintText.GetComponent<Text>().color = TextColour;
                TriggerNameText.text = TriggerName;
                if (IsInActiveHour)
                {
                    InteractHintText.text = InteractHintMessage;
                }
                else
                {
                    InteractHintText.text = OutOfHoursMessage;
                }
            }
            else
            {
                TriggerNameText.GetComponent<Text>().enabled = false;
                InteractHintText.GetComponent<Text>().enabled = false;
            }
        }
    }

    public void HideInteractionText()
    {
        if (TriggerNameText && InteractHintText)
        {
            TriggerNameText.GetComponent<Text>().enabled = false;
            InteractHintText.GetComponent<Text>().enabled = false;
        }
    }
    
    protected void Start()
    {
        GetComponent<Renderer>().enabled = false;
        HideInteractionText();
    }

    protected void Update()
    {
        // Exit on trigger exit key pressed.
        if (IsActivated && ExitPressed())
        {
            Close();
            return;
        }

        // Determine if we're in the active hour. If from and to are flipped the period wraps (e.g. 11pm to 2am).
        if (GameTime)
        {
            float time = GameTime.TimeOfDayHours;
            if (ActiveFromHour < ActiveToHour)
            {
                IsInActiveHour = (time >= ActiveFromHour && time <= ActiveToHour);
            }
            else
            {
                IsInActiveHour = (time >= ActiveFromHour || time <= ActiveToHour);
            }
        }

        // Show prompt allowing the player to activate the trigger.
        if (!IsActivated && IsEnabled && IsPlayerInsideTriggerZone)
        {
            ShowInteractionText();
            if (IsInActiveHour && Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
            {
                IsEnabled = false;
                IsActivated = true;
                HideInteractionText();
                if (onTrigger != null)
                {
                    PlayerState.CurrentTrigger = this;
                    onTrigger();
                }
            }
        }
        else if (IsActivated)
        {
            if (onTriggerUpdate != null)
            {
                onTriggerUpdate();
            }
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerCharacter.gameObject)
        {
            IsPlayerInsideTriggerZone = true;
            if (onPlayerEnter != null)
            {
                onPlayerEnter();
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerCharacter.gameObject)
        {
            IsPlayerInsideTriggerZone = false;
            HideInteractionText();
            if (IsActivated)
            {
                if (onPlayerExit != null)
                {
                    onPlayerExit();
                }
                Close();
            }
        }
    }


}
