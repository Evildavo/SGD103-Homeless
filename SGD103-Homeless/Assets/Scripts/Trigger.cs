using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Trigger : MonoBehaviour
{
    private bool isNearby = false;

    public bool IsActive = true;
    public Text InteractHintText;

    void Start()
    {
        GetComponent<Renderer>().enabled = false;
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
        if (InteractHintText)
        {
            if (IsActive && isNearby)
            {
                InteractHintText.GetComponent<Text>().enabled = true;
            }
            else
            {
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
