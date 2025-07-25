// StackManager.cs
using UnityEngine;

public class StackManager : MonoBehaviour
{
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

    private GameObject currentBlock;

    void Start()
    {
        SpawnNext();
    }

    void Update()
    {
        if (currentBlock != null && Input.GetKeyDown(KeyCode.Space))
            DropCurrent();
    }

    private void SpawnNext()
    {
        // 1) Расположение: прямо над lastBlock на spawnOffsetY
        Vector3 pos = lastBlock.position + Vector3.up * spawnOffsetY;
        pos.x = lastBlock.position.x;
        pos.z = lastBlock.position.z;

        currentBlock = Instantiate(blockPrefab, pos, Quaternion.identity);

        // 2) Двигаем по PingPong
        var mb = currentBlock.GetComponent<MovingBlock>()
                 ?? currentBlock.AddComponent<MovingBlock>();
        mb.moveSpeed = moveSpeed;
        mb.range     = movementRange;
    }

    private void DropCurrent()
    {
        // Остановить PingPong
        currentBlock.GetComponent<MovingBlock>().Stop();
        // Начать контролируемо падать
        var fb = currentBlock.AddComponent<FallingBlock>();
        fb.Initialize(this, fallSpeed);
        currentBlock = null;
    }

    /// <summary>
    /// Вызывается FallingBlock при первом касании земли/других блоков
    /// </summary>
    public void HandleBlockLanding(GameObject landed)
    {
        // 1) Границы предыдущего
        Vector3 prevS = lastBlock.localScale;
        float prevLeft  = lastBlock.position.x - prevS.x/2f;
        float prevRight = lastBlock.position.x + prevS.x/2f;

        // 2) Границы упавшего до обрезки
        float currW     = landed.transform.localScale.x;
        float currLeft  = landed.transform.position.x - currW/2f;
        float currRight = landed.transform.position.x + currW/2f;

        // 3) Вычисляем область перекрытия
        float overlapLeft  = Mathf.Max(prevLeft,  currLeft);
        float overlapRight = Mathf.Min(prevRight, currRight);
        float overlapW     = overlapRight - overlapLeft;

        // 4) Полный промах?
        if (overlapW <= 0f)
        {
            Debug.Log("Game Over!");
            return;
        }

        // 5) Спавним фрагменты по бокам, которые НЕ вошли в overlap
        float leftFragW  = overlapLeft  - currLeft;
        float rightFragW = currRight    - overlapRight;

        if (leftFragW  > 0f) SpawnFragment(leftFragW,  currLeft  + leftFragW/2f, landed);
        if (rightFragW > 0f) SpawnFragment(rightFragW, overlapRight + rightFragW/2f, landed);

        // 6) Обрезаем сам упавший блок под overlapW
        landed.transform.localScale = new Vector3(overlapW, prevS.y, prevS.z);
        landed.transform.position   = new Vector3((overlapLeft + overlapRight)/2f,
                                                  landed.transform.position.y,
                                                  lastBlock.position.z);

        // 7) «Замораживаем» физику основного блока
        var rb = landed.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // 8) Увеличиваем счёт и сложность
        GameManager.Instance.OnBlockPlaced();

        // 9) Обновляем последний блок и спавним следующий
        lastBlock = landed.transform;
        SpawnNext();
    }

    /// <summary>
    /// Создаёт и падает фрагмент ненужной части
    /// </summary>
    private void SpawnFragment(float width, float centerX, GameObject src)
    {
        var frag = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frag.transform.localScale = new Vector3(width, 
                                                src.transform.localScale.y, 
                                                src.transform.localScale.z);
        frag.transform.position   = new Vector3(centerX, 
                                                src.transform.position.y, 
                                                lastBlock.position.z);
        frag.GetComponent<Renderer>().material = src.GetComponent<Renderer>().material;
        frag.AddComponent<Rigidbody>();
    }
}