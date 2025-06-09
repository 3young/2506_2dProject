using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NormarDropItem : MonoBehaviour, IDropItem
{
    [SerializeField] float pickUpRadius = .5f;
    [SerializeField] float attractRadius = 1.5f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] ItemType itemType;

    Transform player;
    Transform boss;

    private void Start()
    {
        player = GameManager.Instance.CurrentPlayer?.transform;
        boss = GameManager.Instance.CurrentBoss?.transform;
    }

    private void Update()
    {
        Transform closest = GetClosestTarget();
        if (closest == null) return;

        float distance = Vector2.Distance(transform.position, closest.position);

        if (distance < attractRadius)
        {
            Vector2 dir = (closest.position - transform.position).normalized;
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        }

        if (distance < pickUpRadius )
        {
            TryPickup(closest);
        }
    }

    private Transform GetClosestTarget()
    {
        float playerDistance = player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;
        float bossDistance = boss ? Vector2.Distance(transform.position, boss.position) : Mathf.Infinity;

        return (playerDistance < bossDistance) ? player : boss;
    }

    private void TryPickup(Transform target)
    {
        if(target.CompareTag("Player"))
        {
            var playerComp = target.GetComponent<Player>();
            if (playerComp != null)
            {
                AbsorbByPlayer(target.GetComponent<Player>());
            }
        }
        else if(target.CompareTag("Cat"))
        {
            var finalBoss = target.GetComponent<FinalBoss>();
            if(finalBoss != null && finalBoss.isActiveAndEnabled)
            {
                AbsorbByBoss(finalBoss);
            }
        }
    }
    private void ApplyEffectToPlayer(Player player)
    {
        switch (itemType)
        {
            case ItemType.Heal:
                float hp = 5f;
                player.GainHp(hp);
                break;

            case ItemType.Experience:
                float exp = 10f;
                player.GainExp(exp);
                break;

            case ItemType.Magnetic:

                break;

            case ItemType.Ban:

                break;
        }
    }


    public void AbsorbByPlayer(Player player)
    {
        ApplyEffectToPlayer(player);
        Destroy(gameObject);
    }

    public void AbsorbByBoss(FinalBoss boss)
    {
        boss.HealPercent(0.3f);
        Destroy(gameObject);
    }
}

public enum ItemType
{
    Heal,
    Experience,
    Magnetic,
    Ban,
}
