using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBoss : Cat
{
    [SerializeField] float attackInterval = 3f;
    [SerializeField] float attackRange = 3f;
    [SerializeField] int attackPower = 5;

    [SerializeField] float buffRange = 5f;
    [SerializeField] float speedBuffAmount = 2f;
    [SerializeField] float buffDuration = 3f;

    private float attackTimer;
    private HashSet<Cat> buffedCats = new HashSet<Cat>();

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

    private void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, buffRange);
        foreach (var hit in hits)
        {
            var cat = hit.GetComponent<Collider2D>();
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
        float newSpeed = originalSpeed + speedBuffAmount;
        cat.SetSpeed(originalSpeed + newSpeed);

        yield return new WaitForSeconds(buffDuration);
        if (cat != null) cat.SetSpeed(originalSpeed);
        buffedCats.Remove(cat);
    }

    public override void Captivated()
    {
        base.Captivated();
        GameEvents.OnBossCaptivated?.Invoke();
    }

}
