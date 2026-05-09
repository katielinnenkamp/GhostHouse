using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class questGiver : Useable
{
    public GameObject dialogBox;
    public Dialogue dialogue;
    public Transform playerCamera;
    public float interactDistance = 3f;

    [SerializeField]
    private Item reward;

    void Start()
    {
        dialogBox.SetActive(false);
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (dialogue.isRunning)
                    {
                        return;
                    }
                    Activate(0);
                }
            }
        }
    }

    public override void Activate(int keyused)
    {
        dialogBox.SetActive(true);
        dialogue.StartDialogue();
        if (reward)
        {
            Instantiate(reward.item_prefab,
                    transform.position + new Vector3(1f, 0f, -1f),
                    Quaternion.identity);
        }
    }
    
    public override void Interact(GameObject Player)
    {
        
    }
}