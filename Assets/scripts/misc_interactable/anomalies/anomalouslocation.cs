using UnityEngine;

public class anomalouslocation : anomalyscript
{
    public Vector3 anomalousloc;
    public Vector3 anomalousrot;

    private Vector3 normalloc;
    private Quaternion normalrot;
    
    void Awake()
    {
        normalloc = transform.localPosition;
        normalrot = transform.localRotation;
    }

    public override void ApplyAnomaly()
    {
        transform.localPosition = anomalousloc;
        transform.localRotation = Quaternion.Euler(anomalousrot);
    }

    public override void RemoveAnomaly()
    {
        transform.localPosition = normalloc;
        transform.localRotation = normalrot;
    }
}
