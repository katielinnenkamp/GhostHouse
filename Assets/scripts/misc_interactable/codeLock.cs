using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class codeLock : Interactable
{
    private bool locked = true;

    [SerializeField]
    private int[] correctcode; //this is the correct code. Can be any 3 numbers between 0 and 9
    [SerializeField]
    private int numberinputs; // the number of inputs on the UI
    private int[] inputtedcode; //currently inputted code on the lock
    public string displayname; //name shown when being interacted with

    [SerializeField]
    private UIDocument lockui; // this is the ui used for the 
    private VisualElement rootve; //root visual element for ui

    // ---- numbers and their related buttons ----
    // TODO; this should be an array because it would mean it could be a dynamic amount, and it would just be simpler
    private Label[] numbers;
    private Button[] incs;
    private Button[] decs;

    //this is the object that this object activates when it unlocks
    [SerializeField]
    private Useable activator;

    void Awake()
    {
        rootve = lockui.rootVisualElement;
        
        rootve.style.display = DisplayStyle.None;

        inputtedcode = new int[numberinputs];
        numbers = new Label[numberinputs];
        incs = new Button[numberinputs];
        decs = new Button[numberinputs];

        for(int i = 0; i < numberinputs; i++)
        {
            int ind = i; // otherwise this is passed by reference in the lambda to inc and dec
            inputtedcode[ind] = 0;

            numbers[ind] = rootve.Q<Label>($"number{ind}");
            incs[ind] = numbers[ind].Q<Button>("increase");
            decs[ind] = numbers[ind].Q<Button>("decrease");
            incs[ind]?.RegisterCallback<ClickEvent>(lambda => Inc(ind));
            decs[ind]?.RegisterCallback<ClickEvent>(lambda => Dec(ind));
        }
    }

    public override void Interact(GameObject Player)
    {
        if(Player.TryGetComponent<playerMove>(out var playerscript))
        {
            if(playerscript.TryEnterMenu(lockui))
            {
                rootve.style.display = DisplayStyle.Flex;
                UpdateUI();
            }
        }
    }
    public override string GetName()
    {
        return displayname;
    }

    //TODO make this an array not a switch
    private void Inc(int numb)
    {
        if(inputtedcode[numb] == 9)
        {
            inputtedcode[numb] = 0;
        }
        else
        {
            inputtedcode[numb]++;
        }

        UpdateUI();

        bool correct = true;
        for(int i = 0; i < numberinputs; i++)
        {
            if(inputtedcode[i] != correctcode[i])
            {
                correct = false;
                break;
            }
        }
        if(correct)
        {
            Unlock();
        }
    }
    private void Dec(int numb)
    {   
        if(inputtedcode[numb] == 0)
        {
            inputtedcode[numb] = 9;
        }
        else
        {
            inputtedcode[numb]--;
        }

        UpdateUI();

        bool correct = true;
        for(int i = 0; i < numberinputs; i++)
        {
            if(inputtedcode[i] != correctcode[i])
            {
                correct = false;
                break;
            }
        }
        if(correct)
        {
            Unlock();
        }
    }

    private void UpdateUI()
    {
        for(int i = 0; i < numberinputs; i++)
        {
            numbers[i].text = inputtedcode[i].ToString();
        }
    }

    private void Unlock()
    {
        Debug.Log("unlocked lock");
        
        activator.Activate(-1);

        Destroy(gameObject);
    }
}
