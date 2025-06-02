using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet2D : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float speed = 1f;

    private BulletPool bulletPool;
    private float lifeTime = 2f;

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
        if(collision.gameObject.layer == LayerMask.NameToLayer("Animal"))
        {
            Destroy(collision.gameObject);
            ReturnToPool();
        }
    }


}
