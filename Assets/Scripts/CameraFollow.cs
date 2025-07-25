// CameraFollow.cs
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public StackManager stackManager;
    public float offsetY = 5f;
    public float smoothSpeed = 2f;

    void Awake()
    {
        if (stackManager == null)
            stackManager = FindObjectOfType<StackManager>();
    }

    void LateUpdate()
    {
        if (stackManager == null || stackManager.lastBlock == null) return;
        Vector3 p = transform.position;
        p.y = stackManager.lastBlock.position.y + offsetY;
        transform.position = Vector3.Lerp(transform.position, p, Time.deltaTime * smoothSpeed);
    }
}
