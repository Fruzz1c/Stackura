using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class StackManager : MonoBehaviour
{
    [Header("Game Control")]
    [Tooltip("Включите, чтобы игра стартовала")]
    public bool enableGame = false;
    private bool gameStarted = false;

    [Header("Префаб и спавн")]
    [Tooltip("Куб-префаб с BoxCollider и Renderer")]
    public GameObject blockPrefab;

    [Tooltip("Последний зафиксированный блок (BaseBlock в сцене)")]
    public Transform lastBlock;

    [Tooltip("На сколько юнитов по Y спавнится новый")]
    public float spawnOffsetY = 5f;

    [Tooltip("Половина диапазона PingPong-движения по X")]
    public float movementRange = 3f;

    [Header("Скорости")]
    [Tooltip("Скорость PingPong-движения")]
    public float moveSpeed = 5f;

    [Tooltip("Скорость падения блока")]
    public float fallSpeed = 5f;

    [Header("Оптимизация")]
    [Tooltip("Максимальное количество сохранённых блоков в сцене")]
    public int maxBlocks = 20;

    private GameObject currentBlock;
    private Queue<GameObject> placedBlocks = new Queue<GameObject>();

    void Update()
    {
        // Если флаг включили впервые во время игры — запускаем
        if (enableGame && !gameStarted)
        {
            gameStarted = true;
            SpawnNext();
        }

        // Пока не стартовали — не обрабатываем ввод
        if (!gameStarted)
            return;

        // Проверяем ввод: Space, левая кнопка мыши или касание экрана
        if (currentBlock != null && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            DropCurrent();
        }
    }

    /// <summary>
    /// Спавнит следующий движущийся блок прямо над lastBlock
    /// </summary>
    private void SpawnNext()
{
    Vector3 pos = lastBlock.position + Vector3.up * spawnOffsetY;
    // после первого успешного приземления (placedBlocks.Count > 0) – рандомим X
    pos.x = placedBlocks.Count > 0
        ? Random.Range(-movementRange, movementRange)
        : lastBlock.position.x;
    pos.z = lastBlock.position.z;

    currentBlock = Instantiate(blockPrefab, pos, Quaternion.identity);

    var mb = currentBlock.GetComponent<MovingBlock>()
             ?? currentBlock.AddComponent<MovingBlock>();
    mb.moveSpeed = moveSpeed;
    mb.range     = movementRange;
}

    /// <summary>
    /// Останавливает PingPong-движение и даёт команду падать
    /// </summary>
    private void DropCurrent()
    {
        currentBlock.GetComponent<MovingBlock>().Stop();
        var fb = currentBlock.AddComponent<FallingBlock>();
        fb.Initialize(this, fallSpeed);
        currentBlock = null;
    }

    /// <summary>
    /// Вызывается FallingBlock при первом касании земли/других блоков.
    /// Возвращает true, если блок перекрыл предыдущий (игра продолжается).
    /// Если перекрытия нет, — Game Over.
    /// </summary>
    public bool HandleBlockLanding(GameObject landed)
    {
        // 1) Границы предыдущего блока
        Vector3 ps = lastBlock.localScale;
        float prevLeft  = lastBlock.position.x - ps.x / 2f;
        float prevRight = lastBlock.position.x + ps.x / 2f;

        // 2) Границы упавшего блока до обрезки
        float cw = landed.transform.localScale.x;
        float currLeft  = landed.transform.position.x - cw / 2f;
        float currRight = landed.transform.position.x + cw / 2f;

        // 3) Область перекрытия
        float overlapLeft  = Mathf.Max(prevLeft,  currLeft);
        float overlapRight = Mathf.Min(prevRight, currRight);
        float overlapW     = overlapRight - overlapLeft;

        // 4) Полный промах?
        if (overlapW <= 0f)
        {
            Debug.Log("Game Over!");
            return false;
        }

        // 5) Спавним фрагменты по бокам, которые не входят в overlap
        float leftFragW  = overlapLeft  - currLeft;
        float rightFragW = currRight    - overlapRight;
        if (leftFragW  > 0f) SpawnFragment(leftFragW,  currLeft  + leftFragW / 2f, landed);
        if (rightFragW > 0f) SpawnFragment(rightFragW, overlapRight + rightFragW / 2f, landed);

        // 6) Обрезаем основной блок под overlapW
        landed.transform.localScale = new Vector3(overlapW, ps.y, ps.z);
        landed.transform.position   = new Vector3((overlapLeft + overlapRight) / 2f,
                                                  landed.transform.position.y,
                                                  lastBlock.position.z);

        // 7) Добавляем в очередь и удаляем старые по превышению maxBlocks
        placedBlocks.Enqueue(landed);
        while (placedBlocks.Count > maxBlocks)
            Destroy(placedBlocks.Dequeue());

        // 8) Уведомляем GameManager и спавним следующий
        GameManager.Instance.OnBlockPlaced();
        lastBlock = landed.transform;
        SpawnNext();

        return true;
    }

    /// <summary>
    /// Создаёт и запускает падающий фрагмент ненужной части
    /// </summary>
    private void SpawnFragment(float width, float centerX, GameObject src)
    {
        var frag = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frag.transform.localScale = new Vector3(width, src.transform.localScale.y, src.transform.localScale.z);
        frag.transform.position   = new Vector3(centerX, src.transform.position.y, lastBlock.position.z);
        frag.GetComponent<Renderer>().material = src.GetComponent<Renderer>().material;
        frag.AddComponent<Rigidbody>();
        frag.AddComponent<FragmentFade>();
    }
}
