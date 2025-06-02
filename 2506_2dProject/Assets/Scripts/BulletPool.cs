using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] Bullet2D bulletPrefab;
    [SerializeField] int poolSize = 20;

    private Queue<Bullet2D> bulletPool = new Queue<Bullet2D>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var bullet = Instantiate(bulletPrefab, transform);
            bullet.gameObject.SetActive(false);
            bulletPool.Enqueue(bullet);
        }

    }

    public Bullet2D GetBullet()
    {
        if (bulletPool.Count > 0)
        {
            var bullet = bulletPool.Dequeue();
            bullet.gameObject.SetActive(true);
            return bullet;
        }
        else
        {
            var bullet = Instantiate(bulletPrefab, transform);
            return bullet;
        }
    }

    public void ReturnBullet(Bullet2D bullet)
    {
        bullet.gameObject.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}
