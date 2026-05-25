using UnityEngine;

public abstract class anomalyscript : MonoBehaviour
{
    private bool anomalous = false;

    void Start()
    {
        anomalymanager.instance.anomalies.Add(this);
    }
    
    public void SetAnomalous()
    {
        if(anomalous)
        {
            return;
        }
        
        anomalous = true;
        ApplyAnomaly();
    }
    public abstract void ApplyAnomaly();

    public void SetNormal()
    {
        if(!anomalous)
        {
            return;
        }

        anomalous = false;
        RemoveAnomaly();
    }
    public abstract void RemoveAnomaly();
}
