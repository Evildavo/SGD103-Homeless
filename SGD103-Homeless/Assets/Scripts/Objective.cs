using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Objective : MonoBehaviour {
    private bool achieved = false;
    private float fadeProgress = 1.0f;
    private float timeAtFadeStart;
    private float initialPanelAlpha;
    private Vector3 initialPosition;

    public Sprite CheckBoxUnchecked;
    public Sprite CheckBoxChecked;
    public Image CheckBox;
    public Text Text;
    public Image Panel;
    
    public bool Achieved = false;
    public float FadeTime;
    public float FadeDistance;

    void Start () {
        initialPanelAlpha = Panel.color.a;
        initialPosition = transform.position;

        startFadeIn();
    }

    void startFadeOut()
    {
        achieved = true;
        CheckBox.sprite = CheckBoxChecked;
        timeAtFadeStart = Time.time;
        fadeProgress = 0.0f;
    }

    void startFadeIn()
    {
        achieved = false;
        CheckBox.sprite = CheckBoxUnchecked;
        timeAtFadeStart = Time.time;
        fadeProgress = 0.0f;
    }

    void Update () {

        // Start fading in.
        if (!Achieved && achieved)
        {
            startFadeIn();
        }

        // Start fading out. 
        if (Achieved && !achieved)
        {
            startFadeOut();
        }

        // Handle fade in.
        if (!achieved && fadeProgress != 1.0f)
        {
            fadeProgress = (Time.time - timeAtFadeStart) / FadeTime;

            // Fade transparency.
            {
                Color colour = CheckBox.color;
                colour.a = fadeProgress;
                CheckBox.color = colour;
            }
            {
                Color colour = Text.color;
                colour.a = fadeProgress;
                Text.color = colour;
            }
            {
                Color colour = Panel.color;
                colour.a = fadeProgress * initialPanelAlpha;
                Panel.color = colour;
            }

            // Fade position.
            {
                Vector3 position = initialPosition;
                position.y += (1.0f - fadeProgress) * FadeDistance;
                transform.position = position;
            }

            // Stop fading when complete.
            if (fadeProgress >= 1.0f)
            {
                fadeProgress = 1.0f;
            }
        }

        // Handle fade out.
        if (achieved && fadeProgress != 1.0f)
        {
            fadeProgress = (Time.time - timeAtFadeStart) / FadeTime;

            // Fade transparency.
            {
                Color colour = CheckBox.color;
                colour.a = (1.0f - fadeProgress);
                CheckBox.color = colour;
            }
            {
                Color colour = Text.color;
                colour.a = (1.0f - fadeProgress);
                Text.color = colour;
            }
            {
                Color colour = Panel.color;
                colour.a = (1.0f - fadeProgress) * initialPanelAlpha;
                Panel.color = colour;
            }

            // Fade position.
            {
                Vector3 position = initialPosition;
                position.y -= fadeProgress * FadeDistance;
                transform.position = position;
            }

            // Destroy when finished fading.
            if (fadeProgress >= 1.0f)
            {
                Destroy(gameObject);
            }
        }
	}
}
