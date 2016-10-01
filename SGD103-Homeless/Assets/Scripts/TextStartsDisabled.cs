using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextStartsDisabled : MonoBehaviour {
    
	void Start () {
        GetComponent<Text>().enabled = false;
	}
}
