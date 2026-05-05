using UnityEngine;
using UnityEngine.InputSystem;


public class GhostText : MonoBehaviour
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
            if (dialogBox.activeSelf == true)
            {
                dialogBox.SetActive(false);
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Ghost"))
                {
                    dialogBox.SetActive(true);
                }
            }
        }
    }
}