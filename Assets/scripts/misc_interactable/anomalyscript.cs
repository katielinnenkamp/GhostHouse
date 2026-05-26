using UnityEngine;

public abstract class anomalyscript : MonoBehaviour
{
    private bool anomalous = false;

    void Start()
    {
        anomalymanager.instance.anomalies.Add(this);
        RemoveAnomaly();
    }
    
    public void SetAnomalous()
    {    
        anomalous = true;
        ApplyAnomaly();
    }
    public abstract void ApplyAnomaly();

    public void SetNormal()
    {
        anomalous = false;
        RemoveAnomaly();
    }
    public abstract void RemoveAnomaly();
}
