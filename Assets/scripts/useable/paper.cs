using UnityEngine;
using UnityEngine.InputSystem;


public class paper : MonoBehaviour
{
    public GameObject dialogBox;
    public Transform playerCamera;
    public float interactDistance = 3f;

    void Start()
    {
        dialogBox.SetActive(false);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (dialogBox.activeSelf)
            {
                dialogBox.SetActive(false);
                return;
            }

            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    dialogBox.SetActive(true);
                }
            }
        }
    }
}
