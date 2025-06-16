using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Transform firePoint;
    [SerializeField] float arrowSpeed = 10f;


    private IObjectPool<Arrow> arrowPool;

    private float lifeTime = 2.5f;
    private int attackPower;
    private bool isReturned = false;

    public void Fire(Vector2 dir, IObjectPool<Arrow> arrowPool, int attackPower)
    {
        //Debug.Log("[Arrow] Fire »£√‚µ ");
        gameObject.SetActive(true);
        rigid.velocity = dir;
        this.arrowPool = arrowPool;
        this.attackPower = attackPower;
        isReturned = false;

        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    private void ReturnToPool()
    {
        //Debug.Log("[Arrow] ReturnToPool »£√‚µ ");

        if (isReturned) return;
        isReturned = true;

        arrowPool.Release(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Cat"))
        {
            if(collision.TryGetComponent<Cat>(out var cat))
            {
                cat.TakeDamage(attackPower);
            }
            ReturnToPool();
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
        rigid.velocity = Vector2.zero;
    }
}
