using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private Cat[] prefabBoss;
    [SerializeField] Transform spawnPoint;

    public Transform target;
    private bool bossSpawned = false;

    private void OnEnable()
    {
        bossSpawned = false;
        var timer = StageTimerManager.Instance;
        if (timer != null)
        {
            timer.OnBossTimeReached.AddListener(SpawnBoss);
        }
    }

    private void OnDisable()
    {
        var timer = StageTimerManager.Instance;
        if (timer != null)
        {
            timer.OnBossTimeReached.RemoveListener(SpawnBoss);
        }
    }

    public void SpawnBoss()
    {
        if (bossSpawned) return;

        int index = Mathf.Clamp(StageController.Instance.currentStage, 0, prefabBoss.Length - 1);
        var obj = Instantiate(prefabBoss[index], spawnPoint.position, Quaternion.identity);
        bossSpawned = true;

        obj.target = target;

        if(obj is FinalBoss finalBoss)
        {
            GameManager.Instance.RegisterBoss(finalBoss);
        }
    }

    public void ResetSpawner()
    {
        bossSpawned = false;
    }

}
