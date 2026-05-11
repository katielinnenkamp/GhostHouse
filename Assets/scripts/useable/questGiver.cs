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
    [SerializeField]
    private GameObject rewardspot;

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
                    dialogBox.SetActive(true);
                    dialogue.StartDialogue();
                }
            }
        }
    }

    public override void Activate(int keyused)
    {
        
        if(reward)
        {
            Instantiate(reward.item_prefab,
                    rewardspot.transform.position,
                    Quaternion.identity);
        }
    }
}