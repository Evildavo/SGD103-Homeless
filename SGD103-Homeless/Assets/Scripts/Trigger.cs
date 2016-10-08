using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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
    public UnityEvent OnTrigger;
    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerExit;

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
        if (IsActive && isNearby)
        {
            ShowInteractionText();
            if (Input.GetKeyDown("e") || Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
            {
                IsActive = false;
                HideInteractionText();
                OnTrigger.Invoke();
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerCharacter.gameObject)
        {
            isNearby = true;
            OnPlayerEnter.Invoke();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerCharacter.gameObject)
        {
            isNearby = false;
            HideInteractionText();
            OnPlayerExit.Invoke();
        }
    }

}
