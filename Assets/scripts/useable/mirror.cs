using UnityEngine;

public class mirror : Useable
{
    [SerializeField]
    private Item hammer;

    [SerializeField]
    private GameObject mirrorBreak;

    [SerializeField]
    private GameObject keyReward;

    public override void Activate(int keyused)
    {
        if(keys[keyused].key == hammer)
        {
            mirrorBreak.SetActive(true);
            keyReward.SetActive(true);
        }
    }
}
