using UnityEngine;
using System.Collections;

public class CharacterDialogueManager : MonoBehaviour {

    public MessageBox MessageBox;

    public bool ShowCaptions = true;
    public bool MustWaitForAudioToFinish = false;
    public bool MustWaitCalculatedLenghtFromText = true;
    public float SecondsPerTextCharacter = 0.08f;

}
