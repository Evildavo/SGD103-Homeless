using UnityEngine;
using System.Collections;

public class CharacterTrigger : Trigger
{
    private MessageBox MessageBox;
    private OnFinishedSpeaking callback;
    private float dialogueLengthTime;
    private float delayAfter;
    private float timeAtStartedSpeaking;
    private bool currentSkippable;
    private bool justStartedSpeaking;

    public CharacterDialogueManager DialogueManager;

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
                      float delayAfterSeconds = 0.0f,
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
        }
        else
        {
            // Calculate from audio.
            dialogueLengthTime = audio.length;
        }

        // Display caption.
        if (DialogueManager.ShowCaptions)
        {
            MessageBox.ShowForTime(text, dialogueLengthTime, gameObject);
        }

        // Play audio.
        if (audio)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = audio;
            audioSource.Play();
        }
    }

    new void Start()
    {
        base.Start();
        MessageBox = DialogueManager.MessageBox;
    }

    new void Update()
    {
        base.Update();

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

    new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    new void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}
