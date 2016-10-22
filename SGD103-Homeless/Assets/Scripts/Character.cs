using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    private OnFinishedSpeaking onFinishedCallback;
    private float dialogueLengthTime;
    private float delayAfter;
    private float timeAtStartedSpeaking;
    private bool currentSkippable;
    private bool justStartedCue;
    private AudioClip audioClip;
    private Queue<CaptionChangeCue> captionChangeCues;
    private CaptionChangeCue? nextCue = null;

    struct CaptionChangeCue
    {
        public float AudioPositionSeconds;
        public string Text;

        public CaptionChangeCue(float audioPositionSeconds, string text)
        {
            AudioPositionSeconds = audioPositionSeconds;
            Text = text;
        }
    }

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
        audioClip = audio;
        onFinishedCallback = callback;
        IsSpeaking = true;
        justStartedCue = true;
        delayAfter = delayAfterSeconds;
        timeAtStartedSpeaking = Time.time;
        currentSkippable = skippable;
        captionChangeCues = new Queue<CaptionChangeCue>();
        nextCue = null;

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
        displayCaption(text);

        // Play audio.
        if (audio)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = audio;
            audioSource.Play();
        }
    }

    // Adds the given cue to be changed at the given time in the audio.
    // Call after Speak().
    public void AddCaptionChangeCue(float audioPositionSeconds, string text)
    {
        captionChangeCues.Enqueue(new CaptionChangeCue(audioPositionSeconds, text));
    }

    void displayCaption(string text)
    {
        if (DialogueManager.ShowCaptions && audioClip != null)
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
    }

    void showNextCaption()
    {
        displayCaption(nextCue.Value.Text);
        if (captionChangeCues.Count > 0)
        {
            nextCue = captionChangeCues.Dequeue();
        }
        else
        {
            nextCue = null;
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
            // Handle cue changes.
            if (captionChangeCues.Count > 0 || nextCue.HasValue)
            {
                // Get the next cue.
                if (!nextCue.HasValue)
                {
                    nextCue = captionChangeCues.Dequeue();
                }

                // Skip on player presses any key.
                if (currentSkippable && Input.anyKeyDown && !justStartedCue)
                {
                    GetComponent<AudioSource>().time = nextCue.Value.AudioPositionSeconds;
                    showNextCaption();
                    justStartedCue = true;
                }
                justStartedCue = false;

                // Change to next cue after enough time has passed.
                if (Time.time - timeAtStartedSpeaking >= nextCue.Value.AudioPositionSeconds)
                {
                    showNextCaption();
                }
            }
            else
            {
                // Skip on player presses any key.
                if (currentSkippable && Input.anyKeyDown && !justStartedCue)
                {
                    GetComponent<AudioSource>().Stop();
                    finishSpeaking();
                }
                justStartedCue = false;

                // Finish speaking after enough time has passed.
                if (Time.time - timeAtStartedSpeaking >= dialogueLengthTime + delayAfter)
                {
                    finishSpeaking();
                }
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
        if (onFinishedCallback != null)
        {
            onFinishedCallback();
        }
    }

}
