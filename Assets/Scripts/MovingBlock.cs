using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    [Tooltip("Скорость PingPong-движения")]
    public float moveSpeed = 5f;
    [Tooltip("Половина диапазона по X от стартовой позиции")]
    public float range = 3f;

    private Vector3 startPos;
    private bool isMoving = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!isMoving) return;
        float x = Mathf.PingPong(Time.time * moveSpeed, range * 2f) - range;
        transform.position = startPos + new Vector3(x, 0f, 0f);
    }

    /// <summary>Остановить движение перед падением</summary>
    public void Stop() => isMoving = false;
}
