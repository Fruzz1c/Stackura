using UnityEngine;

public class BaseBlock : MonoBehaviour
{
    public StackManager stackManager;

    void OnCollisionEnter(Collision other)
    {
        stackManager.enableGame = true;
    }
}
