using Newtonsoft.Json;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum FinalBossState
{
    Waiting,
    Intro,
    Active,
    Affected
}

public class FinalBoss : Cat
{
    [SerializeField] GameObject prefabClearItem;
    [SerializeField] GameObject[] prefabDropItem;
    [SerializeField] float itemDropRadius = 3f;
    [SerializeField] float minSpeed = 0.5f;
    [SerializeField] float wanderInterval = 3f;

    private FinalBossState currentState = FinalBossState.Waiting;

    private ClearItem currentClearItem;
    private float lastDropPercent = 1.0f;

    private Vector3 randomTarget;
    private float wanderTimer;
    private List<Transform> droppedItems = new List<Transform>();

    private BossHPUI bossUI;

    protected override void Start()
    {
        target = null;
        maxHp = 100f;
        currentHp = maxHp;

        bossUI = UIManager.Instance.BossHPUI;
        bossUI.Setup("MINZI", maxHp);

        OnHpChanged.AddListener((currentHp, maxHp) =>
        {
            bossUI.UpdateHP(currentHp);
        });

    }

    public void AppearBoss()
    {
        currentState = FinalBossState.Intro;
        TimelineManager.Instance.PlayBossIntro();
    }

    public void StartBattle()
    {
        currentState = FinalBossState.Active;
    }

    private void Update()
    {
        if (currentState != FinalBossState.Active) return;

        Transform targetItem = GetClosestDroppedItem();

        if (targetItem != null)
        {
            MoveTowards(targetItem.position);

            if(Vector2.Distance(transform.position, targetItem.position) < 0.5f)
            {
                TryAbsorbItem(targetItem.gameObject);
            }
        }
        else
        {
            wanderTimer += Time.deltaTime;

            if (wanderTimer >= wanderInterval || Vector2.Distance(transform.position, randomTarget) < 0.5f)
            {
                randomTarget = GetRandomPositionInCamera();
                wanderTimer = 0f;
            }
            MoveTowards(randomTarget);
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

    private void MoveTowards(Vector3 targetPos)
    {
        Vector2 dir = (targetPos - transform.position).normalized;
        rigid.velocity = dir * speed;

        Vector3 lookDir = targetPos - transform.position;
        lookDir.z = 0;
        transform.right = lookDir.normalized;
        spriteRenderer.flipY = transform.right.x < 0;

    }

    private Vector3 GetRandomPositionInCamera()
    {
        Camera cam = Camera.main;
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0.1f, 0.1f));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(0.9f, 0.9f));

        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);

        return new Vector3(x, y, 0);
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
        currentClearItem = null;
        Captivated();
    }

    public override void Captivated()
    {
        if (currentState == FinalBossState.Affected) return;
        currentState = FinalBossState.Affected;

        bossUI.Hide();

        TimelineManager.Instance.PlayBossClear();
    }

    public void OnClearTimelineEnd()
    {
        GameManager.Instance.WinGame();

        Destroy(gameObject);
    }
}
