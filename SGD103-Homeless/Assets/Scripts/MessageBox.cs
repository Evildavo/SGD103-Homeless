using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageBox : MonoBehaviour
{
    private float fromTime;
    private float closeAfterSeconds;

    [ReadOnly]
    public GameObject Source;

    public void Show(GameObject source)
    {
        gameObject.SetActive(true);
        closeAfterSeconds = 0;
        Source = source;
    }

    public void ShowForTime(float seconds, GameObject source)
    {
        gameObject.SetActive(true);
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
