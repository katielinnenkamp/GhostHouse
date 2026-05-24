using UnityEngine;

public class fireplace : Useable
{
    [SerializeField]
    private Item coal;

    [SerializeField]
    private GameObject fire;

    [SerializeField]
    private GameObject keyReward;

    private int coalCount = 0;
    private bool lit = false;

    void Awake()
    {
        fire.SetActive(false);
    }

    public override void Activate(int keyused)
    {
        if (lit)
            return;

        if(keys[keyused].key == coal)
        {
            coalCount++;

            if(coalCount >= 5)
            {
                lit = true;
                fire.SetActive(true);
                keyReward.SetActive(true);
            }
        }

    }
}
