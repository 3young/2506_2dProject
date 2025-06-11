using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SideHeartPool : MonoBehaviour
{
    public static SideHeartPool Instance { get; private set; }

    [SerializeField] SideHeart bulletPrefab;
    [SerializeField] int defaultCapacity = 100;
    [SerializeField] int maxSize = 150;

    private ObjectPool<SideHeart> pool;
    private readonly HashSet<SideHeart> activeBullets = new HashSet<SideHeart>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        pool = new ObjectPool<SideHeart>(
            CreateFunc,
            OnGet,
            OnRelease,
            OnDestroyPoolObject,
            true,
            defaultCapacity,
            maxSize
        );
    }
    private void Start()
    {
        Preload(50); 
    }

    private void Preload(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = pool.Get();
            pool.Release(obj);
        }
    }

    private SideHeart CreateFunc()
    {
        var heart = Instantiate(bulletPrefab, transform);
        heart.SetPool(pool);
        return heart;
    }

    private void OnGet(SideHeart heart)
    {
        activeBullets.Add(heart);
    }

    private void OnRelease(SideHeart heart)
    {
        heart.gameObject.SetActive(false);
        activeBullets.Remove(heart);
    }

    private void OnDestroyPoolObject(SideHeart heart)
    {
        Destroy(heart.gameObject);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        var bullets = new List<SideHeart>(activeBullets);

        foreach (var heart in activeBullets)
        {
            heart.ManualUpdate(deltaTime);
        }
    }

    public SideHeart GetHeart()
    {
        return pool.Get();
    }

    public void ReturnBullet(SideHeart heart)
    {
        pool.Release(heart);
    }

    public void ClearAllBullets()
    {
        var bulletsToClear = new List<SideHeart>(activeBullets);

        foreach (var bullet in bulletsToClear)
        {
            ReturnBullet(bullet);
        }
    }

    public IObjectPool<SideHeart> Pool => pool;
}