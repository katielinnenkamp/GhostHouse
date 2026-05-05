using UnityEngine;
using UnityEngine.InputSystem;


public class GhostTextEnd : MonoBehaviour
{
    public GameObject dialogBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dialogBox.SetActive(false);
        }
    }
}
