using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] Vector2 velocity = Vector2.zero;
    [SerializeField] float moveSpeed = 1f;

    [SerializeField] float attackSpeed = 1f;
    [SerializeField] int attackPower = 1;
    //[SerializeField] int criticalPower = 2;
    //[SerializeField] float criticalRate = 0.2f;
    [SerializeField] Bullet2D prefabBullet;
    [SerializeField] int maxAmmo = 5;
    [SerializeField] BulletPool bulletPool;
    [SerializeField] Image[] ammoCounts;
    [SerializeField] private float reloadDelay = 1f;

    [SerializeField] Image healthBar;
    [SerializeField] float hp = 30f;
    [SerializeField] float maxHp = 30f;
    [SerializeField] float hpRegenerateSpeed = 0.1f;


    private float attackCooldown = 0f;
    private int currentAmmo = 0;
    private float reloadTimer;
    private bool isFiring;

    private void Start()
    {
        transform.rotation = Quaternion.identity;
        currentAmmo = maxAmmo;
        isFiring = false;
        hp = maxHp;
    }

    private void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        velocity = new Vector2(x, y);
        attackCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && attackCooldown <= 0 && currentAmmo > 0)
        {
            isFiring = true;
            attackCooldown = 1 / attackSpeed;
            currentAmmo--;

            Vector3 mouseWorldPos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = mouseWorldPos - transform.position;
            dir.z = 0;

            var bullet = bulletPool.GetBullet();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir.normalized);
            bullet.Fire(dir.normalized * 5f, bulletPool);
        }
        else
        {
            isFiring = false;

            if (currentAmmo < maxAmmo)
            {
                reloadTimer += Time.deltaTime;

                if (reloadTimer >= reloadDelay)
                {
                    currentAmmo++;
                    reloadTimer = 0f;
                }
            }
        }

        UpdateState();
    }

    private void FixedUpdate()
    {
        rigid.velocity = velocity * moveSpeed;
    }

    private void UpdateState()
    {
        if(Mathf.Approximately(velocity.x, 0) && Mathf.Approximately(velocity.y, 0))
        {
            animator.SetBool("isMove", false);
        }
        else
        {
            animator.SetBool("isMove", true);
        }

        if (velocity.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (velocity.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }

        animator.SetFloat("xDir", velocity.x);
        animator.SetFloat("yDir", velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Animal"))
        {
            hp -= 5f; 
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        healthBar.fillAmount = hp / maxHp;
    }
}
