using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet2D : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Player player;

    private float lifeTime = 2.5f;
    private BulletPool bulletPool;
    private Cat cat;

    public void Fire(Vector2 dir, BulletPool bulletPool)
    {
        rigid.velocity = dir;
        this.bulletPool = bulletPool;

        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    private void ReturnToPool()
    {
        bulletPool.ReturnBullet(this);
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
