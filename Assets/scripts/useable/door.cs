using UnityEngine;

public class door : Useable
{
    public bool opened = false;
    [SerializeField]
    private GameObject hinge;

    public override void Activate(int keyused)
    {
        //unlock door
        no_key = true;

        //stop requiring key
        keys = new key_use_pair[1];

        //open/close door
        if(opened)
        {
            opened = false;
            transform.RotateAround(hinge.transform.position, Vector3.up, 90);
        }
        else
        {
            opened = true;
            transform.RotateAround(hinge.transform.position, Vector3.up, -90);
        }
    }
}
