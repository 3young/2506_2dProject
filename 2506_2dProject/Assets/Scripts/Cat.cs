using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Cat : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Vector2 velocity = Vector2.zero;
    [SerializeField] float speed = 1f;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] int level = 1;
    [SerializeField] float baseHp = 5f;
    [SerializeField] int levelHp = 5;

    [SerializeField] public UnityEvent<float, float> OnHpChanged = new UnityEvent<float, float>();
    [SerializeField] CatHPUI prefabCatHPUI;
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

    private void Start()
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

    }

    private void FixedUpdate()
    {
        if (target == null) return;

        var dir = target.position - transform.position;
        velocity = dir.normalized * speed;
        rigid.velocity = velocity;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);

        spriteRenderer.flipY = targetAngle > 90 || targetAngle < -90;
    }

}
