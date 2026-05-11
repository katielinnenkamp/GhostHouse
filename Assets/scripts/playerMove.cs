using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine.UIElements;

public class playerMove : MonoBehaviour
{
    [SerializeField]
    private float movespeed; //movement speed; can be freely modified and will apply automatically
    [SerializeField]
    private InputActionReference moveref; //movement reference; must be assigned and unchanged
    private Camera cam; //camera; assigned to main camera automatically (should be the only camera in the scene)

    //used in "collide and slide" collision calculations
    private Bounds bounds;

    private float sensitivity = 1f; //sensitivity of mouse movement
    private bool _isLocked = true;

    private UIDocument openedmenu;

    [SerializeField]
    private float gravity = 18f; //gravity; higher gravity feels less floaty
    [SerializeField]
    private float jumppower;
    [SerializeField]
    private float maxairtime;
    private float airtimer = 0f;

    //TODO this shouldn't be public but needs to be for usable objects; find a better way
    public int heldindex = 0;
    private int hotbarslots = 6;
    public Inventory inventory = new Inventory(6); //player inventory tracker, holds current items
    //UI inventory slots
    private VisualElement slot0;
    private VisualElement slot1;
    private VisualElement slot2;
    private VisualElement slot3;
    private VisualElement slot4;
    private VisualElement slot5;
    private VisualElement[] icons = new VisualElement[6];

    private Label helditemlabel;

    private VisualElement itemtag;
    private Label itemtagtext;

    private UIDocument uidoc;
    private VisualElement rootve;
    [SerializeField]
    private GameObject UI;

    [SerializeField]
    private GameObject righthand;
    [SerializeField]
    private GameObject lefthand;

    public MyInputActions controls;
    private AudioManager _audioManager;
    private AudioSource m_Walking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        openedmenu = null;

        bounds = GetComponent<Collider>().bounds;
        bounds.Expand(-2 * skinwidth);

        m_Walking = GameObject.Find("Woodwalk").GetComponent<AudioSource>();
        if (m_Walking == null) Debug.LogError("m_Walking is Null");
        m_Walking.Stop();
        _audioManager = FindFirstObjectByType<AudioManager>();
        if (_audioManager == null) Debug.LogError("_audioManager is NULL");

        yrotation = 0f;
        lookup = 0f;

        cam = Camera.main;

        controls = new MyInputActions();

        controls.Enable();

        itemcolliders = new Collider[maxoverlapitems];

        if(UI.TryGetComponent<UIDocument>(out uidoc))
        {
            rootve = uidoc.rootVisualElement;
            //selectedname = rootve.Q<Label>("selected_name");
            slot0 = rootve.Q<VisualElement>("slot0");
            slot1 = rootve.Q<VisualElement>("slot1");
            slot2 = rootve.Q<VisualElement>("slot2");
            slot3 = rootve.Q<VisualElement>("slot3");
            slot4 = rootve.Q<VisualElement>("slot4");
            slot5 = rootve.Q<VisualElement>("slot5");

            icons[0] = slot0.Q<VisualElement>("icon");
            icons[1] = slot1.Q<VisualElement>("icon");
            icons[2] = slot2.Q<VisualElement>("icon");
            icons[3] = slot3.Q<VisualElement>("icon");
            icons[4] = slot4.Q<VisualElement>("icon");
            icons[5] = slot5.Q<VisualElement>("icon");

            controls.Hotbar.SelectSlot1.performed += lamb => TryChangeHeld(0);
            controls.Hotbar.SelectSlot2.performed += lamb => TryChangeHeld(1);
            controls.Hotbar.SelectSlot3.performed += lamb => TryChangeHeld(2);
            controls.Hotbar.SelectSlot4.performed += lamb => TryChangeHeld(3);
            controls.Hotbar.SelectSlot5.performed += lamb => TryChangeHeld(4);
            controls.Hotbar.SelectSlot6.performed += lamb => TryChangeHeld(5);

            controls.Player.Attack.performed += UseHeld;

            helditemlabel = rootve.Q<Label>("held_item_label");

            itemtag = rootve.Q<VisualElement>("itemtagcontainer");
            itemtagtext = itemtag.Q<Label>("itemtag");
            itemtag.style.display = DisplayStyle.None;
            
            UpdateUI();
        }   
        else
        {
            Debug.Log("UI doc not found...");
        }


    }

    //functions for capturing the cursor and application focus
    // also includes inventory opening/closing function
    #region cursor_and_menues
    public void LockCursor()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        _isLocked = true;
    }
    public void UnlockCursor()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        _isLocked = false;
    }
    // Re-lock when the game regains focus (tab switch, etc.)
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && _isLocked)
            LockCursor();
    }

    void OpenMenu()
    {
        UnlockCursor();
        
        UpdateUI();
    }
    void CloseMenu()
    {
        LockCursor();

        if(openedmenu != null)
        {
            openedmenu.rootVisualElement.style.display = DisplayStyle.None;
            openedmenu = null;
        }

        UpdateUI();
    } 

    //returns true if *not already in menu*
    public bool TryEnterMenu(UIDocument _menu)
    {
        if(!_isLocked)
        {
            return false;
        }
        else
        {
            openedmenu = _menu;
            OpenMenu();
            return true;
        }
    }
    #endregion

    //helper functions
    #region movement_helper_functions
    bool Grounded()
    {
        if(Physics.Raycast(transform.position, Vector3.down, 1.0625f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool HitHead()
    {
        if(Physics.Raycast(transform.position, Vector3.up, 1.0625f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private float movex;
    private float movey;
    void OnMove(InputValue move)
    {
        Vector2 movementVector = move.Get<Vector2>(); 

        movex = movementVector.x;
        movey = movementVector.y;
    }
    private int maxBounces = 5;
    private float skinwidth = 0.015f;
    private Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth = 0)
    {
        if(depth >= maxBounces)
        {
            return Vector3.zero;
        }

        float dist = vel.magnitude + skinwidth;

        RaycastHit hit;

        if(Physics.CapsuleCast(new Vector3(pos.x, pos.y + 0.5f, pos.z), new Vector3(pos.x, pos.y - 0.5f, pos.z), bounds.extents.x, vel.normalized, out hit, dist))
        {
            Vector3 snapToSurface = vel.normalized * (hit.distance - skinwidth);
            Vector3 leftover = vel - snapToSurface;

            if(snapToSurface.magnitude <= skinwidth)
            {
                snapToSurface = Vector3.zero;
            }

            float mag = leftover.magnitude;
            leftover = Vector3.ProjectOnPlane(leftover, hit.normal).normalized;
            leftover *= mag;

            return snapToSurface + CollideAndSlide(leftover, pos + snapToSurface, depth + 1);
        }

        return vel;
    }
    #endregion

    //Update and FixedUpdate; 
    // Update handles input and look among other things
    // FixedUpdate handles movement among other things
    #region standard_updates
    private float yrotation;
    private float lookup;
    private bool groundedonupdate;
    // Update is called once per frame
    void Update()
    {
        
        //wrap in boolean to disable when inside of a menu
        if(_isLocked)
        {
            Vector2 mousedelt = Mouse.current.delta.ReadValue();
            yrotation += mousedelt.x * sensitivity;
            lookup -= mousedelt.y * sensitivity;
            lookup = Mathf.Clamp(lookup, -85f, 85f);
            transform.rotation = Quaternion.Euler(0f, yrotation, 0f);
            cam.transform.rotation = Quaternion.Euler(lookup, yrotation, 0f);
            if(Keyboard.current.eKey.wasPressedThisFrame)
            {
                TryInteract();
            }
            //debug test drop
            if(Keyboard.current.qKey.wasPressedThisFrame)
            {
                DropItem(heldindex);
            }
            groundedonupdate = Grounded();
            //jump
            if(Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if(groundedonupdate || airtimer < maxairtime)
                {
                    airtimer = maxairtime + 1f;
                    vertspeed = jumppower;
                }
            }
            //grounding check
            if(!groundedonupdate)
            {
                m_Walking.Stop();
                if(airtimer <= maxairtime)
                {
                    airtimer += Time.deltaTime;
                }
            }
            else
            {
                airtimer = 0f;
            }
        }
    
        if(Keyboard.current.iKey.wasPressedThisFrame)
        {
            if(!_isLocked)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
        //HandleNumberKeys();
        //HandleScrollWheel();


    }
    private float vertspeed;
    void FixedUpdate()
    {
        InteractableUpdate();
        //get our movement and apply it
        Vector3 movement = new Vector3(movex * Time.deltaTime * movespeed, 0f, movey * Time.deltaTime * movespeed);
        if(!_isLocked)
        {
            if(movement != Vector3.zero)
            {
                CloseMenu();
            }
        }
        movement = Quaternion.Euler(0f, yrotation, 0f) * movement;
        this.transform.position += CollideAndSlide(movement, this.transform.position);
        //if we move, exit out of any menus we're in
        
        // fall if we're in the air, stop falling if we're on the ground
        if(!Grounded())
        {
            vertspeed = 0f - Mathf.Min(-(vertspeed) + (Time.deltaTime * gravity), 55f);
        }
        if(vertspeed != 0f)
        {
            transform.position += CollideAndSlide(new Vector3(0f, (vertspeed * Time.deltaTime), 0f), this.transform.position);
        }
        if(Grounded() && vertspeed <= 0f)
        {
            vertspeed = 0f;
        }
        
        //walking sfx
        bool hasHorizontalInput = !Mathf.Approximately (movex * Time.deltaTime * movespeed, 0f);
        bool hasVerticalInput = !Mathf.Approximately (movey * Time.deltaTime * movespeed, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        if (isWalking)
        {
            if (!m_Walking.isPlaying && Grounded())
            {
                m_Walking.Play();
            }
        }
        else
        {
            m_Walking.Pause();        
        }
    }
    #endregion
    
    //contains everything surrounding items, including interaction and inventory management
    #region item_interaction
    [SerializeField]
    LayerMask interactablelayer;
    float maxinteractdist = 3f;
    Interactable currentinteractable; //currently hovered over interactable
    Interactable lastci = null; //currentinteractable from last frame
    Collider[] itemcolliders;
    int maxoverlapitems = 15;
    void InteractableUpdate()
    {
        lastci = currentinteractable;
        //first we see if the player is directly looking at any interactable object
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, Quaternion.Euler(lookup, yrotation, 0f) * Vector3.forward, out hit, maxinteractdist))
        {
            if(hit.collider.TryGetComponent<Interactable>(out var item)) 
            {
                if(item.enabled)
                {
                    currentinteractable = item;
                }
            }
        }
        //otherwise, we look for the object closest to the crosshair in a capsule in front of the player
        else
        {   
            Vector3 point1 = cam.transform.position + (Quaternion.Euler(lookup, yrotation, 0f) * (Vector3.forward * (maxinteractdist * (1f/3f))));
            Vector3 point2 = cam.transform.position + (Quaternion.Euler(lookup, yrotation, 0f) * (Vector3.forward * (maxinteractdist * (2f/3f))));
            
            Physics.OverlapCapsuleNonAlloc(point1, point2, 1f, itemcolliders);

            Ray ray = new Ray(cam.transform.position, Quaternion.Euler(lookup, yrotation, 0f) * Vector3.forward);

            float shorestsqrdst = float.MaxValue;
            Collider bestfit = null;
            bool iteminview = false;
            //find the object closest to the crosshair
            for(int i = 0; i < maxoverlapitems; i++)
            {
                if(itemcolliders[i] != null)
                {   
                    if(itemcolliders[i].TryGetComponent<Interactable>(out var temp))
                    {
                        //if an items iteract is not marked enabled, do not interact
                        if(temp.enabled)
                        {
                            Vector3 originToPoint = itemcolliders[i].transform.position - ray.origin;

                            // Project originToPoint onto the ray direction.
                            // ray.direction is already normalized in Unity.
                            float projection = Vector3.Dot(originToPoint, ray.direction);

                            // Clamp to 0 so we don't project "behind" the ray origin
                            projection = Mathf.Max(0f, projection);

                            // Find the closest point on the ray to the object
                            Vector3 closestPointOnRay = ray.origin + ray.direction * projection;

                            // squared distance (no sqrt needed)
                            float sqrdst = (itemcolliders[i].transform.position - closestPointOnRay).sqrMagnitude;

                            if(sqrdst < shorestsqrdst)
                            {
                                //check line of sight to object before assigning it
                                Vector3 direction = cam.transform.position - itemcolliders[i].transform.position;
                                if(!Physics.Raycast(itemcolliders[i].transform.position, direction.normalized, out var inbetween, Mathf.Infinity))
                                {
                                    iteminview = true;

                                    shorestsqrdst = sqrdst;
                                    bestfit = itemcolliders[i];
                                }
                                else if(inbetween.collider.gameObject == gameObject)
                                {
                                    iteminview = true;

                                    shorestsqrdst = sqrdst;
                                    bestfit = itemcolliders[i];
                                }
                            }
                        }
                    }
                }
                //clear space in colliders so it doesn't persist
                itemcolliders[i] = null;
            }
            if(!iteminview)
            {
                currentinteractable = null;
            }
            else
            {
                bestfit.TryGetComponent<Interactable>(out var temp);
                currentinteractable = temp;
            }
        }
        
        //if we have a new interactable object, update the item nametag UI
        if(currentinteractable == null)
        {
            itemtag.style.display = DisplayStyle.None;
        }
        else
        {
            itemtag.style.display = DisplayStyle.Flex;
            itemtagtext.text = currentinteractable.GetName();
        }

        if(currentinteractable != null)
        {
            Vector2 screenpos = cam.WorldToScreenPoint(currentinteractable.transform.position);

            var panelSize = uidoc.rootVisualElement.panel.visualTree.layout;

            itemtag.style.left = (screenpos.x / Screen.width) * panelSize.width;
            itemtag.style.bottom = (screenpos.y / Screen.height) * panelSize.height - 10f;
        }
    }

    bool TryInteract()
    {
        if(currentinteractable == null)
        {
            return false;
        }
        else
        {
            currentinteractable.Interact(this.gameObject);
            _audioManager.PlayRandomPickup();
            UpdateUI();
            return true;
        }
        
        /*
        //old code
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, Quaternion.Euler(lookup, yrotation, 0f) * Vector3.forward, out hit, maxinteractdist))
        
        if(hit.collider != null)
        {
            if(hit.collider.TryGetComponent<Pickup>(out var item)) 
            {
                m_Pickup.Play();
                AddToInventory(item);
            }
            else if(hit.collider.TryGetComponent<Useable>(out var obj))
            {
                obj.Interact(heldindex, inventory, this);
            }
            else
            {
                return false;
            }
        }
        return false;
        */
    }

    //TODO update to input manager
    void HandleScrollWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0f)) return;


        // Scroll down = next slot, scroll up = previous, wrapping around
        int next = scroll < 0
            ? (heldindex + 1) % hotbarslots
            : (heldindex - 1 + hotbarslots) % hotbarslots;

        TryChangeHeld(next);
    }

    void DropItem(int ind)
    {
        if(inventory.NumItems() > 0 && !inventory.SlotEmpty(ind))
        {
            Instantiate(RemoveItem(ind).item_prefab, 
                transform.position + (Quaternion.Euler(lookup, yrotation, 0f) * Vector3.forward), 
                Quaternion.identity);
        }
        else
        {
            UpdateUI();
            return;
        }
        UpdateUI();
    }
    //TODO this is public so interactables can take ingredients and keys when you use them, probably come up with a better solution
    public Item RemoveItem(int ind)
    {
        Item ret;
        inventory.TryGetItem(ind, out ret);
        inventory.RemoveItem(ind);
        UpdateUI();
        return ret;
    }
    void AddToInventory(Pickup item)
    {
        item.Grab(inventory);
        UpdateUI();
    }

    public Item GetHeldItem()
    {
        Item ret;
        if(inventory.TryGetItem(heldindex, out ret))
        {
            return ret;
        }
        return null;
    }
    bool TryChangeHeld(int ind)
    {
        if(ind < hotbarslots){
            heldindex = ind;
            UpdateUI();
            return true;
        }
        else{
            return false;
        }
    }

    void UpdateUI()
    {
        //selection updating
        slot0.RemoveFromClassList("slot-selected");
        slot1.RemoveFromClassList("slot-selected");
        slot2.RemoveFromClassList("slot-selected");
        slot3.RemoveFromClassList("slot-selected");
        slot4.RemoveFromClassList("slot-selected");
        slot5.RemoveFromClassList("slot-selected");
        switch(heldindex)
        {
            case 0:
                slot0.AddToClassList("slot-selected");
                break;
            case 1:
                slot1.AddToClassList("slot-selected");
                break;
            case 2:
                slot2.AddToClassList("slot-selected");
                break;
            case 3:
                slot3.AddToClassList("slot-selected");
                break;
            case 4:
                slot4.AddToClassList("slot-selected");
                break;
            case 5:
                slot5.AddToClassList("slot-selected");
                break;
        }
        
        int slots = inventory.GetSlots();
        for(int i = 0; i < slots; i++)
        {
            icons[i].style.backgroundImage = StyleKeyword.None;
        }
        for(int i = 0; i < slots; i++)
        {
            Item temp;
            if(inventory.TryGetItem(i, out temp))
            {
                icons[i].style.backgroundImage = new StyleBackground(temp.icon);
            }
        }
        if(GetHeldItem() != null)
        {
            helditemlabel.text = GetHeldItem().item_name;
        }
        else{
            helditemlabel.text = "";
        }
    }  
    
    void UseHeld(InputAction.CallbackContext ctx)
    {
        Item held = GetHeldItem();
        if(held != null)
        {
            held.Use(gameObject);
        }
        return;
    }
    #endregion
}