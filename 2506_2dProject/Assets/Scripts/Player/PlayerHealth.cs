using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public Image healthBar;
    [SerializeField] float hp = 30f;
    [SerializeField] float maxHp = 30f;
    [SerializeField] float hpRegenerateSpeed = 2f;
    [SerializeField] float regenDelayAfterHit = 2f;
    [SerializeField] GameObject prefabHitEffect;

    private float lastHitTime = -999f;
    private bool isCollidingWithCat = false;
    private Coroutine damageCoroutine;
    private PlayerAttack attack;

    private void Awake()
    {
        hp = maxHp;

        if (healthBar == null && UIManager.Instance != null)
            healthBar = UIManager.Instance.fillHpImage;

        attack = GetComponent<PlayerAttack>();
    }

    public void RegenerateHealth()
    {
        if (healthBar == null) return;

        bool canRegenerate = Time.time - lastHitTime >= regenDelayAfterHit;

        if (canRegenerate && !attack.isFiring && hp < maxHp)
        {
            hp += hpRegenerateSpeed * Time.deltaTime;
            hp = Mathf.Min(hp, maxHp);
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBar == null) return;
        healthBar.fillAmount = hp / maxHp;
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

    public void ResetHealth()
    {
        hp = maxHp;
        UpdateHealthUI();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Cat"))
        {
            if (!isCollidingWithCat)
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

    private IEnumerator CoDamageOverTime()
    {
        while (isCollidingWithCat)
        {
            TakeDamage(3f);
            yield return new WaitForSeconds(1f);
        }
    }

    public void GainHp(float amount)
    {
        hp += amount;
        hp = Mathf.Min(hp, maxHp);
        UpdateHealthUI();
    }
}
