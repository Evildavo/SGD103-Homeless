using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudButtons : MonoBehaviour {
    
    public Transform HudButtonLabel;
    
	void Start()
    {
        HudButtonLabel.gameObject.SetActive(false);
    }

}
