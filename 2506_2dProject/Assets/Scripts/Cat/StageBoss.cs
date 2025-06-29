using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBoss : Cat
{
    [SerializeField] string bossName;
    [SerializeField] float bossBaseHp = 20f;

    [SerializeField] float attackInterval = 3f;
    [SerializeField] float attackRange = 3f;
    [SerializeField] int attackPower = 5;

    [SerializeField] float buffRange = 5f;
    [SerializeField] float speedBuffAmount = 2f;
    [SerializeField] float buffDuration = 3f;

    private float attackTimer;
    private HashSet<Cat> buffedCats = new HashSet<Cat>();

    BossHPUI bossUI;

    protected override void Start()
    {
        base.Start();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.bossAppearSfx);
        maxHp = bossBaseHp * level;
        currentHp = maxHp;
        bossUI = UIManager.Instance.BossHPUI;
        bossUI.Setup(bossName, maxHp);
        
        OnHpChanged.AddListener((currentHp, maxHp) =>
        {
            bossUI.UpdateHP(currentHp);
        });

    }

    private void Update()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer > attackInterval)
        {
            attackTimer = 0;
            PerformAttack();
        }

        ApplyBuffToNearByCats();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    private void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(attackPower);
                }
            }
        }
    }

    private void ApplyBuffToNearByCats()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, buffRange);
        foreach (var hit in hits)
        {
            var cat = hit.GetComponent<Cat>();
            if (cat != null && cat != this && !buffedCats.Contains(cat))
            {
                buffedCats.Add(cat);
                StartCoroutine(BuffCat(cat));
            }
        }
    }

    private IEnumerator BuffCat(Cat cat)
    {
        float originalSpeed = cat.GetSpeed();
        cat.SetSpeed(originalSpeed + speedBuffAmount);

        yield return new WaitForSeconds(buffDuration);
        if (cat != null) cat.SetSpeed(originalSpeed);
        buffedCats.Remove(cat);
    }

    public override void Captivated()
    {
        
        bossUI.Hide();
        GameEvents.OnBossCaptivated?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, buffRange);
    }
}
