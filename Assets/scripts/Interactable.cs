using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact(GameObject Player);
    public abstract string GetName();
}
