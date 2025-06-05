using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] Vector2 velocity = Vector2.zero;
    [SerializeField] float moveSpeed = 3f;

    public void HandleInput()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        velocity = new Vector2(x, y);


        UpdateState();
    }

    private void UpdateState()
    {
        if (Mathf.Approximately(velocity.x, 0) && Mathf.Approximately(velocity.y, 0))
        {
            animator.SetBool("isMove", false);
        }
        else
        {
            animator.SetBool("isMove", true);
        }

        if (velocity.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (velocity.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }

        animator.SetFloat("xDir", velocity.x);
        animator.SetFloat("yDir", velocity.y);
    }

    public void Move()
    {
        rigid.velocity = velocity * moveSpeed;
    }
}
