using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClearItem : MonoBehaviour, IDropItem
{
    [SerializeField] float attractSpeed = 5f;
    [SerializeField] float absorbDelay = 5f;

    private Transform player;
    private Transform boss;
    public bool isCollected = false;
    private float timer = 0f;

    public UnityEvent OnClearItemPicked;
    public UnityEvent OnClearItemAbsorbed;

    public void SetBoss(Transform bossTransform)
    {
        boss = bossTransform;
        timer = 0f;
    }

    private void Start()
    {
        player = GameManager.Instance.CurrentPlayer?.transform;
    }

    private void Update()
    {
        if (isCollected || player == null) return;

        timer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if(distanceToPlayer < 0.5f)
        {
            OnClearItemPicked.Invoke();
            Destroy(gameObject);
            isCollected = true;
            return;
        }

        if(distanceToPlayer < 3f)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(dir * attractSpeed * Time.deltaTime);
        }

        if(timer > absorbDelay && boss != null)
        {
            Vector2 dir = (boss.position - transform.position).normalized;
            transform.position += (Vector3)(dir * attractSpeed * Time.deltaTime);

            if(Vector2.Distance(transform.position, boss.position) < 0.5f)
            {
                OnClearItemAbsorbed.Invoke();
                Destroy(gameObject);
                isCollected = true;
            }
        }
    }

    public void AbsorbByBoss(FinalBoss boss)
    {
        if (isCollected) return;
        isCollected = true;

        OnClearItemAbsorbed?.Invoke();
        Destroy(gameObject);
    }

    public void AbsorbByPlayer(Player player)
    {
        if (isCollected) return;
        isCollected = true;
        
        OnClearItemPicked?.Invoke();
        Destroy(gameObject);

        GameManager.Instance.WinGame();
    }
}
