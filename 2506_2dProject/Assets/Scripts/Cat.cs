using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Cat : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Transform target;
    [SerializeField] Vector2 velocity = Vector2.zero;
    [SerializeField] float speed = 0.5f;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] int level = 1;
    [SerializeField] float baseHp = 5f;
    [SerializeField] int levelHp = 5;

    [SerializeField] public UnityEvent<float, float> OnHpChanged = new UnityEvent<float, float>();
    [SerializeField] CatHPUI prefabCatHPUI;
    private CatHPUI catHPUI;
    public Transform Target {get => target; set => target = value;}
    public float clampedAngle= 0f;
    public float currentHp;
    public float maxHp;

    private void Start()
    {
        maxHp = baseHp + (level - 1) * levelHp;
        currentHp = maxHp;

        catHPUI = Instantiate(prefabCatHPUI, transform.position + Vector3.up * 1.3f, Quaternion.identity);
        catHPUI.SetFollowTarget(transform, Vector3.up * 1.3f);
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
            Die();
        }
    }

    private void Die()
    {
        if(catHPUI != null)
        {
            Destroy(catHPUI.gameObject);
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        var dir = target.position - transform.position;
        velocity = dir.normalized * speed;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);

        if(targetAngle > 90 || targetAngle < -90)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }

    private void FixedUpdate()
    {
        rigid.velocity = velocity;
        transform.rotation = Quaternion.Euler(0, 0, clampedAngle);
    }

}
