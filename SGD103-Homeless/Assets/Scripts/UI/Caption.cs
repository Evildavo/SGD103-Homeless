using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Caption : MonoBehaviour
{
    float fromTime;
    private float closeAfterSeconds;

    public Main Main;


    // Shows the caption for a number of seconds before closing, interrupting any currently shown caption.
    // If seconds is null then the duration is calculated automatically based on the message text.
    public void ShowForTime(string message, float seconds, Color? textColour = null)
    {
        var text = GetComponent<Text>();
        if (textColour.HasValue)
        {
            text.color = textColour.Value;
        }
        else
        {
            text.color = Color.white;
        }
        text.text = message;
        fromTime = Time.time;
        closeAfterSeconds = seconds;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        Hide();
    }

    void Update()
    {
        // Hide after time.
        if (closeAfterSeconds != 0 && Time.time - fromTime > closeAfterSeconds)
        {
            Hide();
        }
    }
}
