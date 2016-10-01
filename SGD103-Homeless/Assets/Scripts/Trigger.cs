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

    void Start()
    {
        GetComponent<Renderer>().enabled = false;
        if (TriggerNameText && InteractHintText)
        {
            TriggerNameText.text = TriggerName;
            InteractHintText.text = InteractHintMessage;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        isNearby = true;
    }

    void OnTriggerExit(Collider other)
    {
        isNearby = false;
    }

    void Update()
    {
        if (TriggerNameText && InteractHintText)
        {
            if (IsActive && isNearby)
            {
                TriggerNameText.GetComponent<Text>().enabled = true;
                InteractHintText.GetComponent<Text>().enabled = true;
            }
            else
            {
                TriggerNameText.GetComponent<Text>().enabled = false;
                InteractHintText.GetComponent<Text>().enabled = false;
            }
        }
    }

    void OnGUI()
    {
        if (IsActive && isNearby && Event.current.keyCode == KeyCode.E)
        { 
            IsActive = false;
        }
    }

}
