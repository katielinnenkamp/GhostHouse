using UnityEngine;

public class paintings : Useable
{
    public Item currentPainting;

    [SerializeField]
    private bedroomPuzzleManager groupManager;

    [SerializeField]
    private Item correctPainting;

    [SerializeField]
    private Transform paintingPosition;

    private GameObject displayedPainting;
    private bool holdingPainting;

    [SerializeField]
    private string defaultName;

    void Awake()
    {
        holdingPainting = false;
        displayedPainting = null;
    }


    void Start()
    {
        groupManager.Incorrect(this);
    }

    public override bool Use(int usedItemInd, Inventory inventory, playerMove user)
    {
        if (!holdingPainting)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                Item used;
                inventory.TryGetItem(usedItemInd, out used);
                if (used == keys[i].key)
                {
                    if (keys[i].consumed)
                    {
                        user.RemoveItem(usedItemInd);
                    }

                    Activate(i);
                    return true;
                }
            }
            return false;
        }

        else
        {
            if(inventory.AddItem(currentPainting))
            {
                groupManager.Incorrect(this);
                Destroy(displayedPainting);
                holdingPainting = false;
                displayname = defaultName;

                return true;
            }
            return false;
        }
    }

    public override void Activate(int keyUsed)
    {
        holdingPainting = true;
        currentPainting = keys[keyUsed].key;
        if(currentPainting == correctPainting)
        {
            groupManager.Correct(this);
        }
        else
        {
            groupManager.Incorrect(this);
        }

        displayedPainting = Instantiate(
            currentPainting.item_prefab,
            paintingPosition.position,
            paintingPosition.rotation
            );

        displayedPainting.transform.SetParent(paintingPosition);
        displayedPainting.transform.Rotate(-90f, 0f, 180f);
        displayname = currentPainting.name;
    }

    public override void Interact(GameObject player)
    {
        player.TryGetComponent<playerMove>(out var playerScript);
        Use(playerScript.heldindex, playerScript.inventory, playerScript);
    }

    public override string GetName()
    {
        return displayname;
    }
}
