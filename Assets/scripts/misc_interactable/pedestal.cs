using UnityEngine;
using System.Collections.Generic;

public class pedestal : Useable
{
    public Item currentbook;

    [SerializeField]
    private libraryPuzzleManager groupmanager;
    [SerializeField]
    private Item correctbook;

    private GameObject dummy;

    private bool holdingbook;

    [SerializeField]
    private string defaultname;

    void Awake()
    {
        holdingbook = false;
        dummy = null;
    }

    void Start()
    {
        groupmanager.Incorrect(this);
    }

    //override Use so that it can pick the book back up when a book is in
    public override bool Use(int useditemind, Inventory inventory, playerMove user)
    {
        if(holdingbook == false)
        {
            for(int i = 0; i < keys.Length; i++)
            {
                Item used;
                inventory.TryGetItem(useditemind, out used);
                if(used == keys[i].key)
                {
                    if(keys[i].consumed)
                    {
                        user.RemoveItem(useditemind);
                    }
                    Activate(i);
                    return true;
                }
            }
            if(no_key)
            {
                Activate(-1);
                return true;
            }   
            
            return false;
        }
        //give the player the book currently held
        else
        {
            if(inventory.AddItem(currentbook))
            {
                groupmanager.Incorrect(this);
                Destroy(dummy);
                displayname = defaultname;
                holdingbook = false;
                return true;
            }
            else
            {
                Debug.Log("inventory full");
                return false;
            }
        }
    }

    public override void Activate(int keyused)
    {
        holdingbook = true;

        //check if we have the right book
        if(keys[keyused].key == correctbook)
        {
            groupmanager.Correct(this);
        }
        else
        {
            groupmanager.Incorrect(this);
        }

        currentbook = keys[keyused].key;

        //place an intert version of the book in the slot
        dummy = Instantiate(currentbook.item_prefab, transform.position, transform.rotation);
        dummy.TryGetComponent<Rigidbody>(out var rb);
        rb.isKinematic = true;
        dummy.TryGetComponent<Pickup>(out var dummyscript);
        dummyscript.enabled = false;

        //change the pedestal's name to the held book
        displayname = dummyscript.GetName();
    }

    public override void Interact(GameObject Player)
    {
        Player.TryGetComponent<playerMove>(out var playerscript);
        Use(playerscript.heldindex, playerscript.inventory, playerscript);
    }

    public override string GetName()
    {
        return displayname;
    }
}