using Newtonsoft.Json;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;


public class FinalBoss : Cat
{
    [SerializeField] GameObject prefabClearItem;
    [SerializeField] GameObject[] prefabDropItem;
    [SerializeField] float itemDropRadius = 3f;
    [SerializeField] float minSpeed = 0.5f;
    [SerializeField] float wanderInterval = 3f;
    [SerializeField] int attackPower = 5;

    private float attackCooldown = 1.5f;
    private float attackTimer = 0f;

    private ClearItem currentClearItem;
    private float lastDropPercent = 1.0f;

    private Vector3 randomTarget;
    private float wanderTimer;
    private List<Transform> droppedItems = new List<Transform>();
    private Vector2 currentVelocity;


    private BossHPUI bossUI;

    protected override void Start()
    {
        baseSpeed = speed;

        maxHp = 100f;
        currentHp = maxHp;

        bossUI = UIManager.Instance.BossHPUI;
        bossUI.Setup("MINZI", maxHp);

        OnHpChanged.AddListener((currentHp, maxHp) =>
        {
            bossUI.UpdateHP(currentHp);
        });

    }


    private void Update()
    {
        Transform targetItem = GetClosestDroppedItem();
        if (targetItem != null)
        {
            float dist = Vector2.Distance(transform.position, targetItem.position);
            if (dist < 0.5f)
            {
                TryAbsorbItem(targetItem.gameObject);
                currentVelocity = Vector2.zero;
            }
            else
            {
                currentVelocity = (targetItem.position - transform.position).normalized * speed;
            }
        }
        else if(target != null)
        {
            float dist = Vector2.Distance(transform.position, target.position);
            if (dist < 0.5f)
            {
                currentVelocity = Vector2.zero;

                if (attackTimer >= attackCooldown)
                {
                    var hp = target.GetComponent<PlayerHealth>();
                    hp.TakeDamage(attackPower);
                   
                    attackTimer = 0f;
                }
            }
            else
            {
                currentVelocity = (target.position - transform.position).normalized * speed;
                attackTimer += Time.deltaTime;
            }
        }
        else
        {
            currentVelocity = Vector2.zero;
        }
    }

    protected override void FixedUpdate()
    {
        rigid.velocity = currentVelocity;

        if (currentVelocity != Vector2.zero)
        {
            transform.right = currentVelocity.normalized;
            spriteRenderer.flipY = transform.right.x < 0;
        }
    }

    private void TryAbsorbItem(GameObject item)
    {
        var drop = item.GetComponent<IDropItem>();
        if (drop != null)
        {
            drop.AbsorbByBoss(this);
            droppedItems.Remove(item.transform);

            if (item == currentClearItem?.gameObject)
                currentClearItem = null;
        }
    }

    private Transform GetClosestDroppedItem()
    {
        if (droppedItems.Count == 0) return null;

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach(var item in droppedItems)
        {
            if (item == null) continue;

            float dist = Vector2.Distance(transform.position, item.position);
            if(dist < minDist)
            {
                minDist = dist;
                closest = item;
            }
        }

        return closest;
    }

    public override void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);

        //Debug.Log($"[FinalBoss] Damage: {damage}, CurrentHP: {currentHp}");

        OnHpChanged.Invoke(currentHp, maxHp);
        UpdateSpeedBasedOnHp();
        HandleRandomItemDrop();
    }

    private void UpdateSpeedBasedOnHp()
    {
        float ratio = currentHp / maxHp;
        speed = Mathf.Max(minSpeed, speed * ratio);
    }

    private void HandleRandomItemDrop()
    {
        float percent = currentHp / maxHp;

        float[] thresholds = { 0.8f, 0.6f, 0.4f, 0.2f, 0.05f };

        foreach(float threshold in thresholds)
        {
            if(lastDropPercent > threshold && percent <= threshold)
            {
                DropRandomItem();
            }
        }
        lastDropPercent = percent;
    }

    private void DropRandomItem()
    {
        Vector3 dropOffset = Random.insideUnitCircle.normalized * itemDropRadius;
        Vector3 dropPosition = transform.position + dropOffset;

        GameObject selectedItem;
        bool isClear = Random.value < 0.25f;

        if(isClear && prefabClearItem != null)
        {
            selectedItem = prefabClearItem;
        }
        else
        {
            int index = Random.Range(0, prefabDropItem.Length);
            selectedItem = prefabDropItem[index];
        }

        GameObject drop = Instantiate(selectedItem, dropPosition, Quaternion.identity);

        var clearItem = drop.GetComponent<ClearItem>();
        if (clearItem != null)
        {
            currentClearItem = clearItem;
            clearItem.SetBoss(this.transform);
            clearItem.OnClearItemAbsorbed.AddListener(OnClearItemAbsorbed);
            clearItem.OnClearItemPicked.AddListener(OnClearItemPicked);
        }
        else
        {
            droppedItems.Add(drop.transform);
        }
    }

    private void OnClearItemAbsorbed()
    {
        HealPercent(0.5f);
        currentClearItem = null;
    }

    public void HealPercent(float percent)
    {
        float amount = maxHp * percent;
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        OnHpChanged.Invoke(currentHp, maxHp);
        UpdateSpeedBasedOnHp();
    }

    private void OnClearItemPicked()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clearSfx);
        currentClearItem = null;
        animator.SetTrigger("Affected");
        Invoke(nameof(Captivated), 5f);
    }

    public override void Captivated()
    {
        bossUI.Hide();
        GameManager.Instance.WinGame();
    }
}
