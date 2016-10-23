using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Objective : MonoBehaviour {
    private bool visible = false;
    private float fadeProgress = 1.0f;
    private float timeAtFadeStart;
    private float initialPanelAlpha;
    private Vector3 targetPosition;
    private Vector3 currentPosition;

    public ObjectiveList ObjectiveList;
    public Transform BasePosition;
    public Sprite CheckBoxUnchecked;
    public Sprite CheckBoxChecked;
    public Image CheckBox;
    public Text Text;
    public Image Panel;

    public string ObjectiveName;
    public bool Disappearing = false;
    public bool Achieved = false;
    public float FadeTime;
    public float FadeDistance;
    public int SlotNum;
    public float YOffsetBetweenObjectives;
    public float SlidePerSecond;

    void Start () {
        initialPanelAlpha = Panel.color.a;
        currentPosition = transform.position;

        updateTargetPosition();
        startFadeIn();

        // Clear the colour at first.
        {
            Color colour = CheckBox.color;
            colour.a = 0.0f;
            CheckBox.color = colour;
        }
        {
            Color colour = Text.color;
            colour.a = 0.0f;
            Text.color = colour;
        }
        {
            Color colour = Panel.color;
            colour.a = 0.0f;
            Panel.color = colour;
        }
    }

    void startFadeOut()
    {
        visible = true;
        timeAtFadeStart = Time.time;
        fadeProgress = 0.0f;
    }

    void startFadeIn()
    {
        visible = false;
        timeAtFadeStart = Time.time;
        fadeProgress = 0.0f;
    }

    void updateTargetPosition()
    {
        var position = BasePosition.position;
        position.y -= YOffsetBetweenObjectives * SlotNum;

        targetPosition = position;
    }

    void Update () {
        Text.text = ObjectiveName;
        updateTargetPosition();

        // Fade out when achieved.
        if (Achieved)
        {
            CheckBox.sprite = CheckBoxChecked;
            Disappearing = true;
        }
        else
        {
            CheckBox.sprite = CheckBoxUnchecked;
        }

        // Move towards target position.
        if (currentPosition != targetPosition)
        {
            var position = currentPosition;
            if (currentPosition.y < targetPosition.y)
            {
                position.y += SlidePerSecond;

                if (position.y >= targetPosition.y)
                {
                    position.y = targetPosition.y;
                }
            }
            else if (currentPosition.y > targetPosition.y)
            {
                position.y -= SlidePerSecond;

                if (position.y <= targetPosition.y)
                {
                    position.y = targetPosition.y;
                }
            }
            currentPosition = position;
        }

        // Start fading in.
        if (!Disappearing && visible)
        {
            startFadeIn();
        }

        // Start fading out. 
        if (Disappearing && !visible)
        {
            startFadeOut();
        }

        // Handle fade in.
        if (!visible && fadeProgress != 1.0f)
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
                Vector3 position = currentPosition;
                position.x -= (1.0f - fadeProgress) * FadeDistance;
                transform.position = position;
            }

            // Stop fading when complete.
            if (fadeProgress >= 1.0f)
            {
                fadeProgress = 1.0f;
            }
        }

        // Handle fade out.
        else if (visible && fadeProgress != 1.0f)
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
                Vector3 position = currentPosition;
                position.x += fadeProgress * FadeDistance;
                transform.position = position;
            }

            // Destroy when finished fading.
            if (fadeProgress >= 1.0f)
            {
                ObjectiveList.RemoveObjective(this);
            }
        }

        // Update position when not fading.
        else
        {
            transform.position = currentPosition;
        }
	}
}
