using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Splash : MonoBehaviour {

    public void Show(Sprite image)
    {
        gameObject.SetActive(true);
        GetComponent<Image>().sprite = image;
        GetComponent<Image>().color = Color.white;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
