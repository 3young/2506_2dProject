using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] public int attackPower = 1;
    //[SerializeField] int criticalPower = 2;
    //[SerializeField] float criticalRate = 0.2f;
    [SerializeField] Arrow prefabArrow;
    [SerializeField] int maxArrow = 5;
    [SerializeField] public ArrowPool arrowPool;
    [SerializeField] public Image[] arrowCounts;
    [SerializeField] private float reloadDelay = 1.5f;

    private float attackCooldown = 0f;
    private int currentArrow = 0;
    private float reloadTimer;
    public bool isFiring;

    private void Start()
    {
        currentArrow = maxArrow;
        UpdateArrowUI();
        isFiring = false;
    }

    public void HandleAttack()
    {
        attackCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && attackCooldown <= 0 && currentArrow > 0)
        {
            isFiring = true;
            attackCooldown = 1 / attackSpeed;
            currentArrow--;
            UpdateArrowUI();

            Vector3 mouseWorldPos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = mouseWorldPos - transform.position;
            dir.z = 0;

            var arrow = arrowPool.GetArrow();
            arrow.transform.position = transform.position;
            arrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir.normalized);
            arrow.Fire(dir.normalized * 5f, arrowPool, attackPower);
        }
        else
        {
            isFiring = false;

            if (!isFiring && currentArrow < maxArrow)
            {
                reloadTimer += Time.deltaTime;

                if (reloadTimer >= reloadDelay)
                {
                    currentArrow++;
                    UpdateArrowUI();
                    reloadTimer = 0f;
                }
            }
        }
    }

    private void UpdateArrowUI()
    {
        for (int i = 0; i < arrowCounts.Length; i++)
        {
            arrowCounts[i].enabled = i < currentArrow;
        }
    }

    public bool IsFiring => isFiring;
}
