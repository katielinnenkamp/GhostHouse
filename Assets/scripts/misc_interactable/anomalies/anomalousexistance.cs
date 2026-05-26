using UnityEngine;

public class anomalousexistance : anomalyscript
{
    public bool existbydefault;

    public override void ApplyAnomaly()
    {
        gameObject.SetActive(!existbydefault);
    }

    public override void RemoveAnomaly()
    {
        gameObject.SetActive(existbydefault);
    }
}