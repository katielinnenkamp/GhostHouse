using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool enabled = true;
    public abstract void Interact(GameObject Player);
    public abstract string GetName();
}
