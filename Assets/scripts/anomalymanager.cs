using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum amstate
{
    IDLE,
    DOORCLOSING,
    DOOROPENING,
    MOVING
}

public class anomalymanager : MonoBehaviour
{
    public static anomalymanager instance;

    public GameObject door;
    public GameObject player;
    private playerMove playerscript;
    [SerializeField]
    private Vector3 doorclosepos;
    [SerializeField]
    private Vector3 dooropenpos;

    public amstate curstate;

    private float movetimer;
    [SerializeField]
    private float timetomove;
    [SerializeField]
    private float doorspeed;

    private int correctguesses = 0;

    public List<anomalyscript> anomalies;

    private bool anomalous = false;

    [SerializeField]
    private bool guaranteeanomalous; //mostly for debugging
    [SerializeField]
    private bool manifestallanomalies;

    void Awake()
    {
        movetimer = 0;
        curstate = amstate.MOVING;
        instance = this;

        #if UNITY_EDITOR
            guaranteeanomalous = guaranteeanomalous;
            manifestallanomalies = manifestallanomalies;
        #else
            guaranteeanomalous = false;
            manifestallanomalies = false;
        #endif
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(curstate)
        {
            case amstate.IDLE:
                break;
            case amstate.DOORCLOSING:
                door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, doorclosepos, doorspeed * Time.deltaTime);
                if((door.transform.localPosition - doorclosepos).sqrMagnitude <= 0.01f)
                {
                    ChangeState(amstate.MOVING);
                }
                break;
            case amstate.DOOROPENING:
                door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, dooropenpos, doorspeed * Time.deltaTime);
                if((door.transform.localPosition - dooropenpos).sqrMagnitude <= 0.01f)
                {
                    ChangeState(amstate.IDLE);
                }
                break;
            case amstate.MOVING:
                movetimer += Time.deltaTime;
                if(movetimer >= timetomove)
                {
                    ChangeState(amstate.DOOROPENING);
                }
                break;
            default:
                break;
        }
    }

    public void CreateAnomaly()
    {
        int ind = Random.Range(0, anomalies.Count - 1);

        anomalies[ind].SetAnomalous();

        if(manifestallanomalies)
        {
            for(int i = 0; i < anomalies.Count; i++)
            {
                anomalies[i].SetAnomalous();
            }
        }
    }


    public bool DetermineAnomalous()
    {
        int r = Random.Range(0, 12);
        if(r <= correctguesses || guaranteeanomalous)
        {
            return true;
        }

        return false;
    }

    public void GoUp()
    {
        if(curstate != amstate.IDLE)
        {
            return;
        }

        if(!anomalous){ correctguesses++; }
        else { correctguesses--; correctguesses = Mathf.Max(0, correctguesses); }

        ChangeState(amstate.DOORCLOSING);
    }

    public void GoDown()
    {
        if(curstate != amstate.IDLE)
        {
            return;
        }

        if(anomalous){ correctguesses++; }
        else { correctguesses--; correctguesses = Mathf.Max(0, correctguesses); }

        ChangeState(amstate.DOORCLOSING);
    }

    public void ChangeState(amstate newstate)
    {
        if(curstate == newstate)
        {
            return;
        }
        switch(newstate)
        {
            case amstate.IDLE:
                break;
            case amstate.DOORCLOSING:
                break;
            case amstate.DOOROPENING:
                break;
            case amstate.MOVING:
                movetimer = 0;

                //whenever we transition to moving, decide if the next floor is anomalous or not
                anomalous = DetermineAnomalous();
                if(!anomalous)
                {
                    Debug.Log("floor is not anomalous");
                    foreach(var anomaly in anomalies)
                    {
                        anomaly.SetNormal();
                    }
                }
                else
                {
                    Debug.Log("floor is anomalous");
                    CreateAnomaly();
                }
                break;
            default:
                break;
        }
        curstate = newstate;
    }
}
