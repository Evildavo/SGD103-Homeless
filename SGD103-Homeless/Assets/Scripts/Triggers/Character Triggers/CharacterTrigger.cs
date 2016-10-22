using UnityEngine;
using System.Collections;

public class CharacterTrigger : Trigger
{
    private MessageBox MessageBox;
    private OnFinishedSpeaking callback;
    private float dialogueLengthTime;
    private float delayAfter;
    private float timeAtStartedSpeaking;

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
        delayAfter = delayAfterSeconds;
        timeAtStartedSpeaking = Time.time;

        // Calculate dialogue length.
        if (DialogueManager.CalculateDialogueLengthFromText || audio == null)
        {
            // Calculate from text.
            dialogueLengthTime = text.Length * DialogueManager.SecondsPerTextCharacter;
            if (audio)
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
        
        // When finished speaking, run the OnFinishedSpeaking callback.
        if (IsSpeaking && Time.time - timeAtStartedSpeaking >= dialogueLengthTime + delayAfter)
        {
            IsSpeaking = false;
            MessageBox.ShowNext();
            if (callback != null)
            {
                callback();
            }
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
