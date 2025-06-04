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
    [SerializeField] public int attackPower = 1;
    //[SerializeField] int criticalPower = 2;
    //[SerializeField] float criticalRate = 0.2f;
    [SerializeField] Arrow prefabArrow;
    [SerializeField] int maxArrow = 5;
    [SerializeField] ArrowPool arrowPool;
    [SerializeField] Image[] arrowCounts;
    [SerializeField] private float reloadDelay = 1.5f;

    [SerializeField] Image healthBar;
    [SerializeField] float hp = 30f;
    [SerializeField] float maxHp = 30f;
    [SerializeField] float hpRegenerateSpeed = 2f;
    [SerializeField] float regenDelayAfterHit = 2f;
    [SerializeField] GameObject prefabHitEffect;


    private float attackCooldown = 0f;
    private int currentArrow = 0;
    private float reloadTimer;
    private bool isFiring;
    private float lastHitTime = -999f;
    private bool isCollidingWithCat = false;
    private Coroutine damageCoroutine;

    private void Start()
    {
        transform.rotation = Quaternion.identity;
        currentArrow = maxArrow;
        isFiring = false;
        hp = maxHp;
    }

    private void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        velocity = new Vector2(x, y);
        attackCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && attackCooldown <= 0 && currentArrow > 0)
        {
            isFiring = true;
            attackCooldown = 1 / attackSpeed;
            currentArrow--;
            UpdateArrowUI();

            Vector3 mouseWorldPos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = mouseWorldPos - transform.position;
            dir.z = 0;

            var arrow = arrowPool.GetArrow();
            arrow.transform.position = transform.position;
            arrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir.normalized);
            arrow.Fire(dir.normalized * 5f, arrowPool);
        }
        else
        {
            isFiring = false;

            if (!isFiring && currentArrow < maxArrow)
            {
                reloadTimer += Time.deltaTime;

                if (reloadTimer >= reloadDelay)
                {
                    currentArrow++;
                    UpdateArrowUI();
                    reloadTimer = 0f;
                }
            }
        }

        UpdateState();
    }

    private void FixedUpdate()
    {
        rigid.velocity = velocity * moveSpeed;
        RegenerateHealth();
    }

    private void UpdateState()
    {
        if (Mathf.Approximately(velocity.x, 0) && Mathf.Approximately(velocity.y, 0))
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Cat"))
        {
            if(!isCollidingWithCat)
            {
                isCollidingWithCat = true;
                damageCoroutine = StartCoroutine(nameof(CoDamageOverTime));
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Cat"))
        {
            isCollidingWithCat = false;
            if (damageCoroutine != null)
            {
                StopCoroutine(nameof(CoDamageOverTime));
                damageCoroutine = null;
            }
        }
    }

    private void UpdateHealthUI()
    {
        healthBar.fillAmount = hp / maxHp;
    }

    private void UpdateArrowUI()
    {
        for (int i = 0; i < arrowCounts.Length; i++)
        {
            arrowCounts[i].enabled = i < currentArrow;
        }
    }

    private void RegenerateHealth()
    {
        bool canRegenerate = Time.time - lastHitTime >= regenDelayAfterHit;

        if(canRegenerate && !isFiring && hp < maxHp)
        {
            hp += hpRegenerateSpeed * Time.deltaTime;
            hp = Mathf.Min(hp, maxHp);
            UpdateHealthUI();
        }
    }

    private IEnumerator CoDamageOverTime()
    {
        while(isCollidingWithCat)
        {
            TakeDamage(3f);
            yield return new WaitForSeconds(1f);
        }
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;
        lastHitTime = Time.time;

        if (hp <= 0)
        {
            Die();
            return;
        }
        Instantiate(prefabHitEffect, transform.position, Quaternion.identity);
        UpdateHealthUI();
    }

    private void Die()
    {
        GameEvents.OnPlayerDie?.Invoke();
        Destroy(gameObject);
    }
}
