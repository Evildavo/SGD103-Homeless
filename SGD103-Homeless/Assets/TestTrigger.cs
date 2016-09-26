using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestTrigger : MonoBehaviour {

    public Text InteractHintText;

    void OnTriggerEnter(Collider other)
    {
        if (InteractHintText)
        {
            InteractHintText.GetComponent<Text>().enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (InteractHintText)
        {
            InteractHintText.GetComponent<Text>().enabled = false;
        }
    }

    void Start () {
	    
	}
	
	void Update () {
	
	}
}
