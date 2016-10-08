using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmationBox : MonoBehaviour {
    private OnConfirmation onConfirmationCallback;

    public Text ConfirmationText;
    public Text AffirmativeButtonText;
    public Text DenialButtonText;

    public delegate void OnConfirmation();
    
    public void Open(
        OnConfirmation onConfirmationCallback,
        string confirmationMessage = "Are you sure?",
        string affirmativeButtonName = "Yes",
        string denialButtonName = "No")
    {
        gameObject.SetActive(true);
        this.onConfirmationCallback = onConfirmationCallback;
        ConfirmationText.text = confirmationMessage;
        AffirmativeButtonText.text = affirmativeButtonName;
        DenialButtonText.text = denialButtonName;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnConfirm()
    {
        Close();
        onConfirmationCallback();
    }

    public void OnDeny()
    {
        Close();
    }

    void Start()
    {
        Close();
    }
}
