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
        public OnCaptionChanged Callback;

        public CaptionChangeCue(float audioPositionSeconds, string text, OnCaptionChanged callback = null)
        {
            AudioPositionSeconds = audioPositionSeconds;
            Text = text;
            Callback = callback;
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
        
    // Callback for when the caption changes on cue.
    public delegate void OnCaptionChanged();

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
        dialogueLengthTime = calculateDialogueLength(text, audio);

        // Display caption.
        displayCaption(text);

        // Play audio.
        if (audio)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = audio;
            audioSource.time = 0.0f;
            audioSource.Play();
        }
    }

    // Adds the given cue to be changed at the given time in the audio.
    // Call after Speak().
    // A callback can be given that's called at the moment we change to this caption.
    public void AddCaptionChangeCue(float audioPositionSeconds, string text, OnCaptionChanged callback = null)
    {
        // Add the cue.
        captionChangeCues.Enqueue(new CaptionChangeCue(audioPositionSeconds, text, callback));

        // Recalculate dialogue length to include the caption.
        dialogueLengthTime = Mathf.Max(
            dialogueLengthTime, 
            audioPositionSeconds + calculateDialogueLength(text, audioClip));
    }

    float calculateDialogueLength(string text, AudioClip audio)
    {
        float length = 0.0f;
        if (DialogueManager.MustWaitCalculatedLenghtFromText || audio == null)
        {
            // Calculate from text.
            length = text.Length * DialogueManager.SecondsPerTextCharacter;
            if (DialogueManager.MustWaitForAudioToFinish && audio)
            {
                length = Mathf.Max(length, audio.length);
            }
            length = Mathf.Max(length, DialogueManager.MinimumCalculatedDialogueTime);
        }
        else
        {
            // Calculate from audio.
            length = audio.length;
        }
        return length;
    }

    void displayCaption(string text)
    {
        if (DialogueManager.ShowCaptions || audioClip == null)
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
        string text = nextCue.Value.Text;
        justStartedCue = true;

        // Run callback function.
        if (nextCue.Value.Callback != null)
        {
            nextCue.Value.Callback();
        }

        // Display the caption.
        displayCaption(text);

        // Get the next caption.
        if (captionChangeCues.Count > 0)
        {
            nextCue = captionChangeCues.Dequeue();
        }
        else
        {
            nextCue = null;
        }
    }

    bool skipPressed()
    {
        if (currentSkippable)
        {
            return (Input.GetButtonDown("Primary") ||
                    Input.GetButtonDown("Secondary") ||
                    Input.GetKeyDown("e") ||
                    Input.GetKeyDown("return") ||
                    Input.GetKeyDown("enter") ||
                    Input.GetKeyDown("space") ||
                    Input.GetKeyDown("escape"));
        }
        else
        {
            return false;
        }
    }

    // Call from derived.
    protected void Start()
    {
        if (DialogueManager)
        {
            MessageBox = DialogueManager.MessageBox;
        }
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
                if (currentSkippable && skipPressed() && !justStartedCue)
                {
                    if (audioClip && nextCue.HasValue)
                    {
                        float cueAudioPosition = nextCue.Value.AudioPositionSeconds;
                        if (cueAudioPosition < audioClip.length)
                        {
                            GetComponent<AudioSource>().time = cueAudioPosition;
                        }
                    }
                    showNextCaption();
                }
                justStartedCue = false;

                // Change to next cue after enough time has passed.
                if (nextCue.HasValue && Time.time - timeAtStartedSpeaking >= nextCue.Value.AudioPositionSeconds)
                {
                    showNextCaption();
                }
            }
            else
            {
                // Skip on player presses any key.
                if (currentSkippable && skipPressed() && !justStartedCue)
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
