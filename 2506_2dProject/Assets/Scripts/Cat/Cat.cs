using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Cat : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rigid;
    [SerializeField] protected Vector2 velocity = Vector2.zero;
    [SerializeField] protected float speed = 1f;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;

    [SerializeField] protected int level = 1;
    [SerializeField] protected float baseHp = 5f;
    [SerializeField] protected int levelHp = 10;
    [SerializeField] protected bool canAttack = true;

    [Header("Buff / Visual")]
    [SerializeField] protected float dodgeChance = 0.3f; 
    [SerializeField] protected GameObject dodgeEffectPrefab; 

    [SerializeField] public UnityEvent<float, float> OnHpChanged = new UnityEvent<float, float>();
    [SerializeField] CatHPUI prefabCatHPUI;

    [SerializeField] GameObject[] prefabItems;

    private CatHPUI catHPUI;
    public Transform target {get; set;}
    public float clampedAngle= 0f;
    public float currentHp;
    public float maxHp;
    protected bool isBuffed = false;

    protected Vector3 originalLocalPos;
    protected float baseSpeed;

    public float GetSpeed() => speed;
    public void SetSpeed(float value)
    {
        speed = value;
    }

    protected virtual void Start()
    {
        baseSpeed = speed;
        if (this is StageBoss == false)
        {
            maxHp = baseHp + (level - 1) * levelHp;
            currentHp = maxHp;
            originalLocalPos = transform.localPosition;
        }

        catHPUI = Instantiate(prefabCatHPUI, transform.position + Vector3.up * 0.8f, Quaternion.identity);
        catHPUI.SetFollowTarget(transform, Vector3.up * 0.8f);
        OnHpChanged.AddListener(catHPUI.UpdateHP);

        OnHpChanged.Invoke(currentHp, baseHp);
    }


    public virtual void TakeDamage(float damage)
    {
        if (isBuffed && Random.value < dodgeChance)
        {
            if (dodgeEffectPrefab != null)
            {
                GameObject fx = Instantiate(dodgeEffectPrefab, transform.position, Quaternion.identity);
                Destroy(fx, 3f);
            }
            return; 
        }

        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);

        OnHpChanged.Invoke(currentHp, maxHp);

        if(currentHp <= 0)
        {
            if (this is StageBoss boss)
            {
                animator.SetTrigger("Affected");
                target = null;
            }
            else
            {
                Captivated();
            }
        }
    }

    public void ResetSpeed()
    {
        speed = baseSpeed;
    }

    public virtual void Captivated()
    {
        if(catHPUI != null)
        {
            Destroy(catHPUI.gameObject);
        }
        GameManager.Instance.totalCatAffected++;
        GameEvents.OnCatCaptivated?.Invoke();
        Destroy(gameObject);

        DropRandomItem();
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        var dir = target.position - transform.position;
        velocity = dir.normalized * speed;
        rigid.velocity = velocity;
        dir.z = 0;
        transform.right = dir.normalized;

        spriteRenderer.flipY = transform.right.x < 0;
    }

    void DropRandomItem()
    {
        if (prefabItems.Length == 0) return;

        int index = Random.Range(0, prefabItems.Length);
        Vector3 dropPos = transform.position;

        int roll = Random.Range(0, 3); 
        if (roll == 0) 
        {
            var item = Instantiate(prefabItems[index], dropPos, Quaternion.identity);
        }
    }

    public void SetBuffed(bool value)
    {
        if (isBuffed == value) return;

        isBuffed = value;

        if (isBuffed)
        {
            speed = baseSpeed * 2f;
        }
        else
        {
            ResetSpeed();
        }
    }

}
