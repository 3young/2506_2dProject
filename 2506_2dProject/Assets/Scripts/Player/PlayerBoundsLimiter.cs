using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundsLimiter : MonoBehaviour
{
    [SerializeField] PolygonCollider2D cameraBounds;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetBounds(PolygonCollider2D bounds)
    {
        this.cameraBounds = bounds;
    }

    private void FixedUpdate()
    {
        if (cameraBounds == null) return;

        Vector2 clampedPos = ClampInsideBounds(transform.position);
        rb.position = clampedPos;
    }

    private Vector2 ClampInsideBounds(Vector2 pos)
    {
        Vector2 closestPoint = cameraBounds.ClosestPoint(pos);
        return closestPoint;
    }
}
