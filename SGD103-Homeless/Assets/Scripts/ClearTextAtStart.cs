using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClearTextAtStart : MonoBehaviour {
    
	void Start () {
        GetComponent<Text>().text = "";
	}
	
	void Update () {
	
	}
}
