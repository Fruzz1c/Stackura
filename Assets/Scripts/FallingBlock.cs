using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    [Tooltip("Скорость падения (ед./сек)")]
    public float fallSpeed = 5f;

    private StackManager manager;
    private Rigidbody rb;
    private bool hasLanded = false;

    /// <summary>Инициализация: передаём менеджер и скорость падения</summary>
    public void Initialize(StackManager mgr, float speed)
    {
        manager = mgr;
        fallSpeed = speed;

        // создаём Rigidbody и отключаем стандартную гравитацию
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        if (rb != null && !hasLanded)
        {
            rb.velocity = Vector3.down * fallSpeed;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (hasLanded || manager == null) return;
        hasLanded = true;

        // «замораживаем» блок на месте
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        // сообщаем менеджеру, что блок сел
        manager.HandleBlockLanding(gameObject);
    }
}
