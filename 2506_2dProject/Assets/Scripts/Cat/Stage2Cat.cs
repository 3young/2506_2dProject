using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Cat : Cat
{
    [SerializeField] private float aimDuration = 1f;      
    [SerializeField] private float waitAfterLock = 0.5f;  
    [SerializeField] private float dashDuration = 1f;   
    [SerializeField] private float dashSpeed = 7f;

    [SerializeField] private GameObject dashIndicatorPrefab;
    [SerializeField] Animator animator;

    private Vector2 lockedDirection;
    private bool isDashing = false;
    private bool isInDashPhase = false;

    private GameObject dashIndicator;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(nameof(CoDashPattern));
    }

    private IEnumerator CoDashPattern()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 5f));

            rigid.velocity = Vector2.zero;
            isInDashPhase = true;

            lockedDirection = Vector2.zero;

            animator.SetTrigger("Aim");
            float timer = 0f;

            if (dashIndicatorPrefab != null)
            {
                dashIndicator = Instantiate(dashIndicatorPrefab, transform.position, Quaternion.identity);
            }


            while(timer < aimDuration)
            {
                if (target != null)
                {
                    lockedDirection = (target.position - transform.position).normalized;
                    transform.right = lockedDirection;

                    if (dashIndicator != null)
                    {
                        float angle = Mathf.Atan2(lockedDirection.y, lockedDirection.x) * Mathf.Rad2Deg;
                        dashIndicator.transform.rotation = Quaternion.Euler(0,0,angle);
                    }
                }
                timer += Time.deltaTime;
                yield return null;
            }

            if(dashIndicator != null) Destroy(dashIndicator);

            yield return new WaitForSeconds(waitAfterLock);

            isDashing = true;
            float dashTime = 0f;
            while(dashTime < dashDuration)
            {
                rigid.velocity = lockedDirection * dashSpeed;
                dashTime += Time.deltaTime;
                yield return null;
            }

            rigid.velocity = Vector2.zero;
            isDashing = false;
            isInDashPhase = false;
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing && !isInDashPhase && target != null)
        {
            var dir = target.position - transform.position;
            velocity = dir.normalized * speed;
            rigid.velocity = velocity;

            transform.right = dir.normalized;
            spriteRenderer.flipY = transform.right.x < 0;
        }
    }
}
