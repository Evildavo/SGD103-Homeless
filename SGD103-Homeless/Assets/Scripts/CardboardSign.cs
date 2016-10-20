using UnityEngine;
using System.Collections;

public class CardboardSign : MonoBehaviour {
    
    public Material CanvasMaterial;
    public Renderer Canvas;

    public void Show()
    {
        Canvas.enabled = true;
    }

	void Start ()
    {
        Canvas.enabled = false;
    }
	
	void Update ()
    {
    }
}
