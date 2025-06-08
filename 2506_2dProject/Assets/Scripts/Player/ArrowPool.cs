using UnityEngine;
using UnityEngine.Pool;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance { get; private set; }

    [SerializeField] Arrow arrowPrefab;
    [SerializeField] int defaultCapacity = 20;
    [SerializeField] int maxSize = 40;

    private ObjectPool<Arrow> pool;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        pool = new ObjectPool<Arrow>
        (
            CreateFunc,
            OnGet,
            OnRelease,
            OnDestroyPoolObject,
            true,
            defaultCapacity,
            maxSize
        );
    }

    private Arrow CreateFunc()
    {
        var arrow = Instantiate(arrowPrefab, transform);
        return arrow;
    }

    private void OnGet(Arrow arrow)
    {
        arrow.gameObject.SetActive(true);
    }

    private void OnRelease(Arrow arrow)
    {
        arrow.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(Arrow arrow)
    {
        Destroy(arrow.gameObject);
    }

    public Arrow GetArrow()
    {
        return pool.Get();
    }

    public void ReturnArrow(Arrow arrow)
    {
        pool.Release(arrow);
    }

    public IObjectPool<Arrow> Pool => pool;
}
