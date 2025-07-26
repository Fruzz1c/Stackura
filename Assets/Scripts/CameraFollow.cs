using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Ссылка на ваш StackManager")]
    public StackManager stackManager;

    [Header("Отступы")]
    [Tooltip("Вертикальный отступ камеры над последним блоком")]
    public float offsetY = 5f;
    [Tooltip("Горизонтальный отступ камеры относительно башни")]
    public float offsetX = 0f;

    [Header("Скорость сглаживания")]
    [Tooltip("Чем больше, тем быстрее камера догоняет цель")]
    public float smoothSpeed = 2f;

    void Awake()
    {
        if (stackManager == null)
            stackManager = FindObjectOfType<StackManager>();
    }

    void LateUpdate()
    {
        if (stackManager == null || stackManager.lastBlock == null) return;

        // Вычисляем целевую позицию камеры с учётом отступов
        Vector3 targetPos = new Vector3(
            stackManager.lastBlock.position.x + offsetX,
            stackManager.lastBlock.position.y + offsetY,
            transform.position.z  // оставляем свое Z
        );

        // Плавно перемещаем камеру к targetPos
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothSpeed * Time.deltaTime
        );
    }
}