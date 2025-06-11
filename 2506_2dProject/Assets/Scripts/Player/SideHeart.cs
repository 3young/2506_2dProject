using UnityEngine;
using UnityEngine.Pool;

public class SideHeart : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float lifeTime = 1.5f;

    private float damage;
    private Vector2 direction;
    private float timer;

    private IObjectPool<SideHeart> pool;


    public void Initialize(Vector2 dir, float dmg)
    {
        direction = dir.normalized;
        damage = dmg;
        timer = 0f;
    }

    public void ManualUpdate(float deltaTime)
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            ReturnToPool();
        }
    }

    public void SetPool(IObjectPool<SideHeart> pool)
    {
        this.pool = pool;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cat"))
        {
            if (collision.TryGetComponent<Cat>(out Cat cat))
            {
                cat.TakeDamage(damage);
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        if (!gameObject.activeSelf) return;

        pool?.Release(this);
    }
}