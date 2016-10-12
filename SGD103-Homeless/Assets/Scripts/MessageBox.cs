using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageBox : MonoBehaviour
{
    private float fromTime;
    private float closeAfterSeconds;

    [ReadOnly]
    public GameObject Source;

    public void Show(string message, GameObject source)
    {
        gameObject.SetActive(true);
        GetComponentInChildren<Text>().text = message;
        closeAfterSeconds = 0;
        Source = source;
    }

    public void ShowForTime(string message, float seconds, GameObject source)
    {
        gameObject.SetActive(true);
        GetComponentInChildren<Text>().text = message;
        Source = source;
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
        if (closeAfterSeconds != 0 && Time.time - fromTime > closeAfterSeconds)
        {
            Hide();
        }
    }
}
