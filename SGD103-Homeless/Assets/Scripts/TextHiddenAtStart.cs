using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextHiddenAtStart : MonoBehaviour {
    
	void Start () {
        GetComponent<Text>().enabled = false;
	}
	
}
