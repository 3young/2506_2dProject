using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Arrow : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Player player;

    private float lifeTime = 2.5f;
    private ArrowPool bulletPool;
    private Cat cat;

    public void Fire(Vector2 dir, ArrowPool bulletPool)
    {
        rigid.velocity = dir;
        this.bulletPool = bulletPool;

        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    private void ReturnToPool()
    {
        bulletPool.ReturnArrow(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Cat"))
        {
            if(collision.TryGetComponent<Cat>(out cat))
            {
                cat.TakeDamage(player.attackPower);
            }
            ReturnToPool();
        }
    }


}
