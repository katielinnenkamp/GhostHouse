using UnityEngine;
using System.Collections.Generic;

public class elevatorbutton : Interactable
{
    public bool goesup;

    [SerializeField]
    private string name;

    public override void Interact(GameObject Player)
    {
        Player.TryGetComponent<playerMove>(out var playerscript);

        if(goesup){anomalymanager.instance.GoUp();}
        else{anomalymanager.instance.GoDown();}
    }

    public override string GetName()
    {
        return name;
    }
}