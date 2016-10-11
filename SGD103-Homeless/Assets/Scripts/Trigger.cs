using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Trigger : MonoBehaviour
{

    // The first paramater is the trigger that called the event.
    [System.Serializable]
    public class TriggerEvent : UnityEvent<Trigger> { }

    public PlayerCharacter PlayerCharacter;
    public GameTime GameTime;
    public Text TriggerNameText;
    public Text InteractHintText;

    public bool IsEnabled = true;
    [ReadOnly]
    public bool IsPlayerInsideTriggerZone = false;
    [ReadOnly]
    public bool IsActivated = false;
    public string TriggerName;
    public string InteractHintMessage;

    [Tooltip("Called when the player activates the trigger.")]
    public TriggerEvent OnTrigger;

    [Tooltip("Called while the trigger is activated.")]
    public TriggerEvent OnTriggerUpdate;

    [Tooltip("Called when the player enters the trigger zone.")]
    public TriggerEvent OnPlayerEnter;

    [Tooltip("Called when the player exists the trigger zone.")]
    public TriggerEvent OnPlayerExit;

    // Resets the trigger after being triggered. Returns game-time speed to normal.
    // If enabled is false the player won't be able to reactivate the trigger.
    public void Reset(bool enabled = true)
    {
        IsEnabled = enabled;
        IsActivated = false;
        GameTime.IsTimeAccelerated = false;
    }
    
    public void ShowInteractionText()
    {
        if (TriggerNameText && InteractHintText)
        {
            TriggerNameText.GetComponent<Text>().enabled = true;
            InteractHintText.GetComponent<Text>().enabled = true;
            TriggerNameText.text = TriggerName;
            InteractHintText.text = InteractHintMessage;
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
    
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
        HideInteractionText();
    }
    
    void Update()
    {
        // Show prompt allowing the player to activate the trigger.
        if (!IsActivated && IsEnabled && IsPlayerInsideTriggerZone)
        {
            ShowInteractionText();
            if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
            {
                IsEnabled = false;
                IsActivated = true;
                GameTime.IsTimeAccelerated = true;
                HideInteractionText();
                OnTrigger.Invoke(GetComponent<Trigger>());
            }
        }
        else if (IsActivated)
        {
            OnTriggerUpdate.Invoke(GetComponent<Trigger>());
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerCharacter.gameObject)
        {
            IsPlayerInsideTriggerZone = true;
            OnPlayerEnter.Invoke(GetComponent<Trigger>());
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerCharacter.gameObject)
        {
            IsPlayerInsideTriggerZone = false;
            HideInteractionText();
            OnPlayerExit.Invoke(GetComponent<Trigger>());
        }
    }

}
