using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageBox : MonoBehaviour
{
    private float fromTime;
    private float closeAfterSeconds;

    public GameObject Source;

    public void Show(GameObject source)
    {
        Source = source;
        gameObject.SetActive(true);
    }

    public void ShowForTime(float seconds, GameObject source)
    {
        Source = source;
        gameObject.SetActive(true);
        fromTime = Time.time;
        closeAfterSeconds = seconds;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsDisplayed()
    {
        return gameObject.activeInHierarchy;
    }

    public void SetMessage(string message)
    {
        GetComponentInChildren<Text>().text = message;
    }
    
    void Start () {
        Hide();
    }
	
	void Update () {
        if (Time.time - fromTime > closeAfterSeconds)
        {
            Hide();
        }
    }
}
