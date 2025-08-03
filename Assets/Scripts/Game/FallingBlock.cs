// FallingBlock.cs
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    [Tooltip("Скорость падения (units/sec)")]
    public float fallSpeed = 10f;

    private StackManager manager;
    private Rigidbody rb;
    private bool triggered = false;
    private float fallTimer = 0f;

    /// <summary>Передаём ссылку на менеджер и скорость падения</summary>
    public void Initialize(StackManager mgr, float speed)
    {
        manager = mgr;
        fallSpeed = speed;

        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;

        // Замораживаем вращение по всем осям
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (triggered) return;

        // Двигаем вниз с постоянной скоростью
        rb.velocity = Vector3.down * fallSpeed;
    }

    void Update()
    {
        if (triggered) return;

        // Считаем время падения
        fallTimer += Time.deltaTime;
        if (fallTimer >= 1f)
        {
            GameManager.Instance.ResetGame();
            // по таймауту — конец игры
            Debug.Log("Game Over! (timeout)");
            triggered = true;
            Destroy(this);
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (triggered || manager == null) return;
        triggered = true;

        bool placed = manager.HandleBlockLanding(gameObject);

        if (placed)
        {
            rb.velocity    = Vector3.zero;
            rb.isKinematic = true;
        }
        else
        {
            GameManager.Instance.ResetGame();
            // промах — оставляем динамическим, пусть падает дальше
            Debug.Log("Game Over! (miss)");
        }

        Destroy(this);
    }
}