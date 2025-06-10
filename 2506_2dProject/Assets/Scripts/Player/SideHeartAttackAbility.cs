using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SideHeartAttackAbility : MonoBehaviour
{
    [SerializeField] GameObject sideHeartPrefab;
    [SerializeField] float fireInterval = 5f;
    [SerializeField] float damageMultiplier = 0.5f; 
    [SerializeField] Transform firePoint;

    private int level = 1;
    private PlayerAttack playerAttack;

    private Coroutine firingRoutine;

    private void Start()
    {
        var player = GameManager.Instance.CurrentPlayer.GetComponent<Player>();
        playerAttack = player.GetComponent<Player>().Attack;
    }

    private void OnEnable()
    {
        if (firingRoutine == null)
            firingRoutine = StartCoroutine(FireLoop());
    }

    private void OnDisable()
    {
        if (firingRoutine != null)
        {
            StopCoroutine(firingRoutine);
            firingRoutine = null;
        }
    }

    private IEnumerator FireLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireInterval);
            FireBullets();
        }
    }

    private void FireBullets()
    {
        List<float> angles = GetAnglesByLevel(level);
        float damage = playerAttack.attackPower * damageMultiplier;

        foreach (float angle in angles)
        {
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            SideHeart heart = SideHeartPool.Instance.GetHeart();
            heart.transform.position = firePoint.position;
            heart.transform.rotation = rot;
            heart.Initialize(rot * Vector2.right, damage);
        }
    }

    private List<float> GetAnglesByLevel(int lvl)
    {
        List<float> result = new List<float>();

        for (int i = 1; i <= lvl; i++)
        {
            float angle = (30f * i) % 360f;
            result.Add(angle);
        }

        return result;
    }

    public void LevelUp()
    {
        level = Mathf.Clamp(level + 1, 1, 12);
    }
}