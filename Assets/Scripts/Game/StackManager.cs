using UnityEngine;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    [Header("Game Control")]
    public bool enableGame = false;
    private bool gameStarted = false;

    [Header("Префаб и спавн")]
    public GameObject blockPrefab;
    public Transform lastBlock;
    public float spawnOffsetY  = 5f;
    public float movementRange = 3f;

    [Header("Скорости")]
    public float moveSpeed = 5f;
    public float fallSpeed = 5f;

    [Header("Оптимизация")]
    public int   maxBlocks = 20;

    [Header("Толерантность")]
    [Tooltip("Допустимое смещение (в долях от ширины) для идеального попадания")]
    [Range(0f, 0.1f)]
    public float placementTolerance = 0.1f; // 1%

    private GameObject currentBlock;
    private Queue<GameObject> placedBlocks = new Queue<GameObject>();

    void Update()
    {
        if (enableGame && !gameStarted)
        {
            gameStarted = true;
            SpawnNext();
        }
        if (!gameStarted) return;

        if (currentBlock != null &&
           (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetMouseButtonDown(0)    ||
            Input.touchCount > 0))
        {
            DropCurrent();
        }
    }

    private void SpawnNext()
    {
        Vector3 pos = lastBlock.position + Vector3.up * spawnOffsetY;
        pos.x = lastBlock.position.x;
        pos.z = lastBlock.position.z;

        currentBlock = Instantiate(blockPrefab, pos, Quaternion.identity);
        // копируем размер предыдущего
        currentBlock.transform.localScale = lastBlock.localScale;

        var mb = currentBlock.GetComponent<MovingBlock>()
                 ?? currentBlock.AddComponent<MovingBlock>();
        mb.moveSpeed = moveSpeed;
        mb.range     = movementRange;
    }

    private void DropCurrent()
    {
        currentBlock.GetComponent<MovingBlock>().Stop();
        var fb = currentBlock.AddComponent<FallingBlock>();
        fb.Initialize(this, fallSpeed);
        currentBlock = null;
    }

    /// <summary>
    /// Возвращает true, если блок успешно установлен (игра продолжается).
    /// </summary>
    public bool HandleBlockLanding(GameObject landed)
    {
        Vector3 prevScale = lastBlock.localScale;
        float prevHalfW   = prevScale.x / 2f;
        float hangover    = landed.transform.position.x - lastBlock.position.x;
        float absHang     = Mathf.Abs(hangover);

        // Полный промах?
        if (absHang >= prevScale.x)
        {
            Debug.Log("Game Over!");
            return false;
        }

        // Проверяем на толерантность (идеальное попадание)
        float tol = prevScale.x * placementTolerance;
        if (absHang <= tol)
        {
            // центрируем и оставляем тот же размер
            landed.transform.localScale = prevScale;
            landed.transform.position   = new Vector3(
                lastBlock.position.x,
                landed.transform.position.y,
                lastBlock.position.z);

            // «Замораживаем» физику
            //var rb = landed.GetComponent<Rigidbody>();
            //if (rb != null) rb.isKinematic = true;

            EnqueueAndCleanup(landed);
            return true;
        }

        // Обычная обрезка
        float overlapW = prevScale.x - absHang;
        float centerX  = lastBlock.position.x + hangover / 2f;

        // спавним фрагменты
        float leftFragW  = (landed.transform.position.x - prevHalfW) - (centerX - overlapW/2f);
        float rightFragW = (centerX + overlapW/2f) - (landed.transform.position.x + prevHalfW);
        if (hangover > 0f) // с правой стороны
            SpawnFragment(absHang, centerX + overlapW/2f + absHang/2f, landed);
        else              // с левой стороны
            SpawnFragment(absHang, centerX - overlapW/2f - absHang/2f, landed);

        // обрезаем основной блок
        landed.transform.localScale = new Vector3(overlapW, prevScale.y, prevScale.z);
        landed.transform.position   = new Vector3(centerX,
                                                  landed.transform.position.y,
                                                  lastBlock.position.z);

        // «Замораживаем» его
        //var rb2 = landed.GetComponent<Rigidbody>();
        //if (rb2 != null) rb2.isKinematic = true;

        EnqueueAndCleanup(landed);
        return true;
    }

    private void EnqueueAndCleanup(GameObject landed)
    {
        // уведомляем GameManager
        GameManager.Instance.OnBlockPlaced();

        // обновляем очередь и удаляем старейшие
        placedBlocks.Enqueue(landed);
        while (placedBlocks.Count > maxBlocks)
            Destroy(placedBlocks.Dequeue());

        // обновляем последний и спавним новый
        lastBlock = landed.transform;
        SpawnNext();
    }

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
