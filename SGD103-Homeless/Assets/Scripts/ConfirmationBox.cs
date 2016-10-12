using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ConfirmationBox : MonoBehaviour {
    private OnChoiceMade onChoiceMadeCallback;
    private bool isMouseOver = false;

    public Text ConfirmationText;
    public Text AffirmativeButtonText;
    public Text DenialButtonText;

    public delegate void OnChoiceMade(bool yes);
    
    public void Open(
        OnChoiceMade onChoiceMadeCallback,
        string confirmationMessage = "Are you sure?",
        string affirmativeButtonName = "Yes",
        string denialButtonName = "No")
    {
        gameObject.SetActive(true);
        this.onChoiceMadeCallback = onChoiceMadeCallback;
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
        onChoiceMadeCallback(true);
    }

    public void OnDeny()
    {
        Close();
        onChoiceMadeCallback(false);
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
