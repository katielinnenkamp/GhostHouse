using UnityEngine;

[System.Serializable]
public struct key_use_pair
{
    public Item key;
    public bool consumed;
}

public abstract class Useable : Interactable
{
    [SerializeField]
    protected key_use_pair[] keys;
    [SerializeField]
    protected bool no_key;
    [SerializeField]
    protected string displayname;

    public bool Use(int useditemind, Inventory inventory, playerMove user)
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

    public abstract void Activate(int keyused);

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