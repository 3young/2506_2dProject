using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] int maxMultiArrowCount = 5; 
    [SerializeField] Transform firePoint; 
    [SerializeField] float arrowSpeed = 5f;

    [SerializeField] private PlayerStats playerStats;

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

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            isFiring = true;
            attackCooldown = 1f / attackSpeed;

            int playerLevel = playerStats != null ? playerStats.Level : 1;
            int arrowsToFire = Mathf.Min(playerLevel, maxMultiArrowCount);
            currentArrow -= 1;
            UpdateArrowUI();

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = mouseWorldPos - firePoint.position;
            dir.z = 0;
            dir.Normalize();

            Vector3 perpendicular = Vector3.Cross(dir, Vector3.forward).normalized;


            for (int i = 0; i < arrowsToFire; i++)
            {
                float offset = (i - (arrowsToFire - 1) / 2f) * 0.5f;
                Vector3 spawnPos = firePoint.position + perpendicular * offset;

                var arrow = arrowPool.GetArrow();
                if (arrow == null)
                {
                    continue;
                }

                arrow.transform.position = spawnPos;
                arrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
                arrow.Fire(dir * arrowSpeed, arrowPool.Pool, attackPower);
            }
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
