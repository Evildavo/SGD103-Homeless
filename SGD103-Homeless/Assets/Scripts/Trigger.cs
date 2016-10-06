using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Trigger : MonoBehaviour
{
    private bool isNearby = false;

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
        isNearby = true;
    }

    // Call from derived.
    public virtual void OnTriggerExit(Collider other)
    {
        isNearby = false;
        HideInteractionText();
    }

}
