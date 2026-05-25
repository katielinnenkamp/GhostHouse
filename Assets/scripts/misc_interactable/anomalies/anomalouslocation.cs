using UnityEngine;

public class anomalouslocation : anomalyscript
{
    public Vector3 anomalousloc;
    public Vector3 anomalousrot;

    private Vector3 normalloc;
    private Quaternion normalrot;
    
    void Awake()
    {
        normalloc = this.transform.localPosition;
        normalrot = this.transform.localRotation;
    }

    public override void ApplyAnomaly()
    {
        gameObject.transform.localPosition = anomalousloc;
        gameObject.transform.localRotation = Quaternion.Euler(anomalousrot);
    }

    public override void RemoveAnomaly()
    {
        gameObject.transform.localPosition = normalloc;
        gameObject.transform.rotation = normalrot;
    }
}
