using UnityEngine;
using System.Collections;

public class Ambience : MonoBehaviour {

    public Main Main;
    public float SecondsFade = 2.0f;
    [Range(0.0f, 24.0f)]
    public float NightFrom = 19.0f;
    [Range(0.0f, 24.0f)]
    public float NightTo = 4.0f;

    public void Resume()
    {
        foreach (AudioSource audio in GetComponents<AudioSource>())
        {
            audio.UnPause();
        }
    }

    public void Pause()
    {
        foreach (AudioSource audio in GetComponents<AudioSource>())
        {
            audio.Pause();
        }
    }

    void UpdateAudio()
    {
        // Change based on time of day.
        AudioSource[] audioSources = GetComponents<AudioSource>();
        AudioSource dayAudio = audioSources[0];
        AudioSource nightAudio = audioSources[1];

        // Night.
        if (Main.GameTime.TimeOfDayHours > NightFrom || 
            Main.GameTime.TimeOfDayHours < NightTo)
        {
            if (dayAudio.volume > 0.0f)
            {
                dayAudio.volume -= (1.0f / SecondsFade) * Time.deltaTime;
            }
            else
            {
                dayAudio.volume = 0.0f;
            }
            if (nightAudio.volume < 1.0f)
            {
                nightAudio.volume += (1.0f / SecondsFade) * Time.deltaTime;
            }
            else
            {
                nightAudio.volume = 1.0f;
            }
        }

        // Day.
        else
        {
            if (dayAudio.volume < 1.0f)
            {
                dayAudio.volume += (1.0f / SecondsFade) * Time.deltaTime;
            }
            else
            {
                dayAudio.volume = 1.0f;
            }
            if (nightAudio.volume > 0.0f)
            {
                nightAudio.volume -= (1.0f / SecondsFade) * Time.deltaTime;
            }
            else
            {
                nightAudio.volume = 0.0f;
            }
        }
    }

	void Start ()
    {
        UpdateAudio();
        foreach (AudioSource audio in GetComponents<AudioSource>())
        {
            audio.Play();
        }
    }
	
	void Update ()
    {
        UpdateAudio();
	}
}
