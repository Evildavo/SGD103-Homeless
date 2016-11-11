using UnityEngine;
using System.Collections;

public class Ambience : MonoBehaviour {

    public AudioClip Day;
    public AudioClip Night;

    public void Resume()
    {
        var audio = GetComponent<AudioSource>();
        audio.UnPause();
    }

    public void Pause()
    {
        var audio = GetComponent<AudioSource>();
        audio.Pause();
    }

	void Start ()
    {
        var audio = GetComponent<AudioSource>();
        audio.clip = Day;
        audio.Play();
    }
	
	void Update () {
	
	}
}
