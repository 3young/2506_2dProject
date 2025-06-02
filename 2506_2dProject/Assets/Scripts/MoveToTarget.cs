using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] Transform target;
    [SerializeField] Vector2 velocity = Vector2.zero;
    [SerializeField] float speed = 0.5f;
    [SerializeField] SpriteRenderer spriteRenderer;

    public Transform Target {get => target; set => target = value;}
    public float clampedAngle= 0f;

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
