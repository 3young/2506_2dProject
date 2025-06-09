using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3Cat : Cat
{
    [SerializeField] float buffRange = 5f;
    [SerializeField] float buffInterval = 0.2f;
    [SerializeField] float speedBuffMultiplier = 2f;
    [SerializeField] SpriteRenderer buffRangeVisual;

    [SerializeField] LayerMask catLayer;

    private HashSet<Cat> buffedCats = new();

    protected override void Start()
    {
        base.Start();
        canAttack = false;
        StartCoroutine(BuffCoroutine());

        if (buffRangeVisual != null)
        {
            float diameter = buffRange;
            buffRangeVisual.transform.localScale = new Vector3(diameter, diameter, 1f);
        }
    }

    IEnumerator BuffCoroutine()
    {
        while (true)
        {
            ApplyBuffToNearbyCats();
            yield return new WaitForSeconds(buffInterval);
        }
    }

    void ApplyBuffToNearbyCats()
    {
        Debug.Log("버프 적용 시도");
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, buffRange, catLayer);
        HashSet<Cat> currentCatsInRange = new();

        foreach (var hit in hits)
        {
            Cat cat = hit.GetComponent<Cat>();
            if (cat != null && cat != this)
            {
                currentCatsInRange.Add(cat);

                if (!buffedCats.Contains(cat))
                {
                    cat.SetBuffed(true);
                    buffedCats.Add(cat);
                }
            }
        }

        foreach (var cat in buffedCats)
        {
            if (!currentCatsInRange.Contains(cat))
            {
                cat.SetBuffed(false);
                cat.ResetSpeed();
            }
        }

        buffedCats = currentCatsInRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, buffRange);
    }
}