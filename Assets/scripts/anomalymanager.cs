using UnityEngine;
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
    public AnomalyText anomalytext;
    private bool first = true;

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
    [SerializeField]
    private int currentfloor = 6;

    [SerializeField]
    private int winfloor = 0;

    [SerializeField]
    private int losefloor = 9;

    public List<anomalyscript> anomalies = new List<anomalyscript>();

    private bool anomalous = false;

    [SerializeField]
    private bool guaranteeanomalous; //mostly for debugging
    [SerializeField]
    private bool manifestallanomalies;

    [SerializeField]
    private int recentAnomalyMemory = 3;

    private List<anomalyscript> recentlyUsedAnomalies = new List<anomalyscript>();

    void Awake()
    {
        movetimer = 0;
        curstate = amstate.IDLE;
        instance = this;
        door.transform.localPosition = doorclosepos;

    #if !UNITY_EDITOR
        guaranteeanomalous = false;
        manifestallanomalies = false;
    #endif
    }

    void Start()
    {
        ShowIntro();
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
                    anomalytext.ShowMessage("Current Floor: B" + currentfloor);
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
        }
    }

    public void CreateAnomaly()
    {
        if (manifestallanomalies)
        {
            for (int i = 0; i < anomalies.Count; i++)
            {
                anomalies[i].SetAnomalous();
            }
            return;
        }
        List<anomalyscript> possibleAnomalies = new List<anomalyscript>();

        for (int i = 0; i < anomalies.Count; i++)
        {
            if (!recentlyUsedAnomalies.Contains(anomalies[i]))
            {
                possibleAnomalies.Add(anomalies[i]);
            }
        }
        if (possibleAnomalies.Count == 0)
        {
            recentlyUsedAnomalies.Clear();
            possibleAnomalies.AddRange(anomalies);
        }

        int ind = Random.Range(0, possibleAnomalies.Count);
        anomalyscript chosenAnomaly = possibleAnomalies[ind];
        chosenAnomaly.SetAnomalous();
        recentlyUsedAnomalies.Add(chosenAnomaly);
        while(recentlyUsedAnomalies.Count > recentAnomalyMemory)
        {
            recentlyUsedAnomalies.RemoveAt(0);
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

        bool correct = !anomalous;
        HandleGuess(correct);
    }

    public void GoDown()
    {
        if(curstate != amstate.IDLE)
        {
            return;
        }

        bool correct = anomalous;
        HandleGuess(correct);
    }

    private void HandleGuess(bool correct)
    {
        if (correct)
        {
            currentfloor--;
            correctguesses++;
        }
        else
        {
            currentfloor++;
            correctguesses--;
            correctguesses = Mathf.Max(0, correctguesses);
        }

        

        Debug.Log("Currrent floor: B" + currentfloor);

        if (currentfloor <= winfloor)
        {
            WinGame();
            return;
        }

        if (currentfloor >= losefloor)
        {
            LoseGame();
            return;
        }

        ChangeState(amstate.DOORCLOSING);
    }

    private void WinGame()
    {
        Debug.Log("You reached B0, you win!");
        curstate = amstate.IDLE;
    }

    private void LoseGame()
    {
        Debug.Log("You reached B9, you lose!");
        curstate = amstate.IDLE;
    }

    private void ShowIntro()
    {
        anomalytext.ShowMessage(
            "Welcome to the anomaly zone. \n" +
            "You start on Basement level 6, take a look around. \n" +
            "The first run through is safe, so try to remember everything. \n" +
            "If everything looks normal, press the safe button in the elevator, but if you find an anomaly, press the anomaly button instead. \n" +
            "Every correct guess brings you closer to the surface, make it to B0 and you are free! \n" +
            "But guess incorrectly and fall to B9, you fail and never escape. \n\n" +
            "Good luck, you'll need it.",
            StartFirstFloor
        );
    }

    private void StartFirstFloor()
    {
        ChangeState(amstate.MOVING);
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

                foreach(var anomaly in anomalies)
                {
                    anomaly.SetNormal();
                }

                if (first)
                {
                    anomalous = false;
                    first = false;
                    Debug.Log("First floor, no anomalies");
                }
                else
                {
                    //whenever we transition to moving, decide if the next floor is anomalous or not
                    anomalous = DetermineAnomalous();
                    if(!anomalous)
                    {
                        Debug.Log("floor is not anomalous");
                    }
                    else
                    {
                        Debug.Log("floor is anomalous");
                        CreateAnomaly();
                    }
                }
                break;
        }
        curstate = newstate;
    }
}
