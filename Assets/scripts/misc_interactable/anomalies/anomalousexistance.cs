using UnityEngine;

public class anomalousexistance : anomalyscript
{
    public bool existbydefault;
    private MeshRenderer renderer;
    private Collider collide;
    
    void Awake()
    {
        collide = GetComponent<Collider>();
        renderer = GetComponent<MeshRenderer>();
        if(!existbydefault)
        {
            collide.enabled = false;
            renderer.enabled = false;
        }
    }

    public override void ApplyAnomaly()
    {
        if(existbydefault)
        {
            collide.enabled = false;
            renderer.enabled = false;
        }
        else
        {
            collide.enabled = true;
            renderer.enabled = true;
        }
    }

    public override void RemoveAnomaly()
    {
        if(!existbydefault)
        {
            collide.enabled = false;
            renderer.enabled = false;
        }
        else
        {
            collide.enabled = true;
            renderer.enabled = true;
        }
    }
}
