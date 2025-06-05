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

        var obj = Instantiate(prefabBoss[StageController.Instance.stage], spawnPoint.position, Quaternion.identity);
        bossSpawned = true;

        obj.target = target;
    }

    public void ResetSpawner()
    {
        bossSpawned = false;
    }

}
