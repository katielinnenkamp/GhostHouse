using UnityEngine;
using TMPro;
using System;

public class AnomalyText : MonoBehaviour
{
    public TMP_Text popupText;

    private bool showingMessage = false;
    private Action onClose;

    void Awake()
    {
        popupText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (showingMessage && Input.GetMouseButtonDown(0))
        {
            HideMessage();
        }
    }

    public void ShowMessage(string message)
    {
        popupText.text = message;
        popupText.gameObject.SetActive(true);

        showingMessage = true;
        onClose = null;
    }

    public void ShowMessage(string message, Action closeAction)
    {
        popupText.text = message;
        popupText.gameObject.SetActive(true);

        showingMessage = true;
        onClose = closeAction;
    }

    public void HideMessage()
    {
        popupText.gameObject.SetActive(false);
        showingMessage = false;

        if (onClose != null)
        {
            onClose.Invoke();
            onClose = null;
        }
    }
}