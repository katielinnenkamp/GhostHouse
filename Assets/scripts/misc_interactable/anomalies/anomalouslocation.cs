using UnityEngine;

public class anomalouslocation : anomalyscript
{
    public Vector3 anomalousloc;
    public Vector3 anomalousrot;

    private Vector3 normalloc;
    private Quaternion normalrot;
    
    void Awake()
    {
        normalloc = this.transform.position;
        normalrot = this.transform.rotation;
    }

    public override void ApplyAnomaly()
    {
        gameObject.transform.position = anomalousloc;
        gameObject.transform.rotation = Quaternion.Euler(anomalousrot);
    }

    public override void RemoveAnomaly()
    {
        gameObject.transform.position = normalloc;
        gameObject.transform.rotation = normalrot;
    }
}
