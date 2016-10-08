using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ConfirmationBox : MonoBehaviour {
    private OnConfirmation onConfirmationCallback;
    private bool isMouseOver = false;

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

    public void OnEnter()
    {
        isMouseOver = true;
    }

    public void OnExit()
    {
        isMouseOver = false;
    }

    void Start()
    {
        Close();
    }

    void Update()
    {
        if (Input.GetButtonDown("Primary") && !isMouseOver)
        {
            OnDeny();
        }
        if (Input.GetKeyDown("enter") || Input.GetKeyDown("return"))
        {
            OnConfirm();
        }
        else if (Input.GetKeyDown("escape"))
        {
            OnDeny();
        }
    }
}
