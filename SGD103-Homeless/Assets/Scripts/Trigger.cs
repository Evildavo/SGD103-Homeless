using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Trigger : MonoBehaviour
{
    private bool isNearby = false;

    public PlayerCharacter PlayerCharacter;
    public Text TriggerNameText;
    public Text InteractHintText;

    public bool IsActive = true;
    public string TriggerName;
    public string InteractHintMessage;
    
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

    // Override to do some action on being triggered.
    public virtual void OnTrigger()
    {
        IsActive = true;
    }
    
    // Override to do some action when the player enters the trigger.
    public virtual void OnPlayerEnter() {}

    // Override to do some action when the player enters the trigger.
    public virtual void OnPlayerExit() {}

    // Call from derived.
    public virtual void Start()
    {
        GetComponent<Renderer>().enabled = false;
        HideInteractionText();
    }

    // Call from derived.
    public virtual void Update()
    {
        if (IsActive && isNearby)
        {
            ShowInteractionText();
            if (Input.GetKeyDown("e") || Input.GetKeyDown("enter"))
            {
                IsActive = false;
                HideInteractionText();
                OnTrigger();
            }
        }
    }

    // Call from derived.
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerCharacter.gameObject)
        {
            isNearby = true;
            OnPlayerEnter();
        }
    }

    // Call from derived.
    public virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerCharacter.gameObject)
        {
            isNearby = false;
            HideInteractionText();
            OnPlayerExit();
        }
    }

}
