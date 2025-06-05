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

    [SerializeField] int level = 1;
    [SerializeField] float baseHp = 5f;
    [SerializeField] int levelHp = 5;

    [SerializeField] public UnityEvent<float, float> OnHpChanged = new UnityEvent<float, float>();
    [SerializeField] CatHPUI prefabCatHPUI;

    [SerializeField] GameObject[] prefabItems;

    private CatHPUI catHPUI;
    public Transform target {get; set;}
    public float clampedAngle= 0f;
    public float currentHp;
    public float maxHp;

    public float GetSpeed() => speed;
    public void SetSpeed(float value)
    {
        speed = value;
    }

    protected virtual void Start()
    {
        maxHp = baseHp + (level - 1) * levelHp;
        currentHp = maxHp;

        catHPUI = Instantiate(prefabCatHPUI, transform.position + Vector3.up * 0.8f, Quaternion.identity);
        catHPUI.SetFollowTarget(transform, Vector3.up * 0.8f);
        OnHpChanged.AddListener(catHPUI.UpdateHP);

        OnHpChanged.Invoke(currentHp, baseHp);
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);

        OnHpChanged.Invoke(currentHp, maxHp);

        if(currentHp <= 0)
        {
            Captivated();
        }
    }

    public virtual void Captivated()
    {
        if(catHPUI != null)
        {
            Destroy(catHPUI.gameObject);
        }

        GameEvents.OnCatCaptivated?.Invoke();
        Destroy(gameObject);

        DropRandomItem();
    }

    private void FixedUpdate()
    {
        if (target == null) return;

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
        Instantiate(prefabItems[index], transform.position, Quaternion.identity);
    }

}
