using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance { get; private set; }

    [SerializeField] Arrow arrowPrefab;
    [SerializeField] int defaultCapacity = 50;
    [SerializeField] int maxSize = 100;

    private ObjectPool<Arrow> pool;
    private readonly List<Arrow> activeArrows = new List<Arrow>();


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

        if (!activeArrows.Contains(arrow))
        {
            activeArrows.Add(arrow);
        }
    }

    private void OnRelease(Arrow arrow)
    {
        arrow.gameObject.SetActive(false);

        activeArrows.Remove(arrow);
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

    public void ClearAllArrows()
    {
        for (int i = activeArrows.Count - 1; i >= 0; i--)
        {
            ReturnArrow(activeArrows[i]);
        }
    }

    public IObjectPool<Arrow> Pool => pool;
}
