using UnityEngine;

public class door : Useable
{
    public bool opened = false;
    [SerializeField]
    private GameObject hinge;
    private AudioManager _audioManager;

    void Awake()
    {
        _audioManager = FindFirstObjectByType<AudioManager>();
    }

    public override void Activate(int keyused)
    {
        //unlock door
        no_key = true;

        //stop requiring key
        keys = new key_use_pair[1];

        //open/close door
        if(opened)
        {
            _audioManager.DoorClopen(opened);
            opened = false;
            transform.RotateAround(hinge.transform.position, Vector3.up, 90);
        }
        else
        {
            _audioManager.DoorClopen(opened);
            opened = true;
            transform.RotateAround(hinge.transform.position, Vector3.up, -90);
        }
    }
}
