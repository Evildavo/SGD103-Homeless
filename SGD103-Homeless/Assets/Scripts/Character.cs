using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    private OnFinishedSpeaking callback;
    private float dialogueLengthTime;
    private float delayAfter;
    private float timeAtStartedSpeaking;
    private bool currentSkippable;
    private bool justStartedSpeaking;

    public CharacterDialogueManager DialogueManager;
    [ReadOnly]
    public MessageBox MessageBox;

    public string SpeakerName;
    public Color SpeakerTextColour = Color.white;

    [ReadOnly]
    public bool IsSpeaking = false;

    // Callback for when dialogue has finished being spoken.
    public delegate void OnFinishedSpeaking();

    // Speaks dialogue. 
    // Text is the written transcript, used for captions.
    // Callback is called when the the text has finished being spoken.
    // An optional delay can be added after the audio finishes.
    public void Speak(string text,
                      AudioClip audio = null,
                      OnFinishedSpeaking callback = null,
                      float delayAfterSeconds = 0.15f,
                      bool skippable = true)
    {
        this.callback = callback;
        IsSpeaking = true;
        justStartedSpeaking = true;
        delayAfter = delayAfterSeconds;
        timeAtStartedSpeaking = Time.time;
        currentSkippable = skippable;

        // Calculate dialogue length.
        if (DialogueManager.MustWaitCalculatedLenghtFromText || audio == null)
        {
            // Calculate from text.
            dialogueLengthTime = text.Length * DialogueManager.SecondsPerTextCharacter;
            if (DialogueManager.MustWaitForAudioToFinish && audio)
            {
                dialogueLengthTime = Mathf.Max(dialogueLengthTime, audio.length);
            }
            dialogueLengthTime = Mathf.Max(dialogueLengthTime, DialogueManager.MinimumCalculatedDialogueTime);
        }
        else
        {
            // Calculate from audio.
            dialogueLengthTime = audio.length;
        }

        // Display caption.
        if (DialogueManager.ShowCaptions)
        {
            string message = "";
            if (SpeakerName != "")
            {
                message = SpeakerName + ": " + text;
            }
            else
            {
                message = text;
            }
            MessageBox.ShowForTime(message, dialogueLengthTime, gameObject, false, SpeakerTextColour);
        }

        // Play audio.
        if (audio)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = audio;
            audioSource.Play();
        }
    }

    // Call from derived.
    protected void Start()
    {
        MessageBox = DialogueManager.MessageBox;
    }

    // Call from derived.
    protected void Update()
    {
        // Wait to finish speaking.
        if (IsSpeaking)
        {
            // Skip on player presses any key.
            if (currentSkippable && Input.anyKeyDown && !justStartedSpeaking)
            {
                GetComponent<AudioSource>().Stop();
                finishSpeaking();
            }
            justStartedSpeaking = false;

            // Finish speaking after enough time has passed.
            if (Time.time - timeAtStartedSpeaking >= dialogueLengthTime + delayAfter)
            {
                finishSpeaking();
            }
        }
    }

    void finishSpeaking()
    {
        IsSpeaking = false;

        // Close the message box.
        if (MessageBox.IsDisplayed())
        {
            MessageBox.ShowNext();
        }

        // Run the OnFinishedSpeaking callback.
        if (callback != null)
        {
            callback();
        }
    }

}
