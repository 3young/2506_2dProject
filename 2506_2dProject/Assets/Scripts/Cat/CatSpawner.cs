using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CatSpawner : MonoBehaviour
{
    [SerializeField] Cat[] prefabsCat;
    [SerializeField] float baseSpawnInterval = 3f;

    public Transform target;

    public float spawnInterval;
    public float spawnIntervalDecay = 0.95f;

    private float difficultyTimer = 0f;
    private float difficultyInterval = 10f;

    private int stage;
    private bool isBossSpawned = false;

    private Coroutine spawnCoroutine;

    private void Start()
    {
        spawnInterval = baseSpawnInterval;

        if (StageController.Instance != null)
        {
            StageController.Instance.OnStageChanged.AddListener(UpdateStage);
        }

        if(StageTimerManager.Instance != null)
        {
            StageTimerManager.Instance.OnBossTimeReached.AddListener(HandleBossSpawned);
        }
    }
    private void OnDisable()
    {
        if (StageController.Instance != null)
        {
            StageController.Instance.OnStageChanged.RemoveListener(UpdateStage);
        }
    }

    private void Update()
    {
        if (isBossSpawned) return;

        difficultyTimer += Time.deltaTime;
        if (difficultyTimer > difficultyInterval)
        {
            spawnInterval *= spawnIntervalDecay;
            difficultyTimer = 0f;
        }
    }


    public void UpdateStage(int newStage)
    {
        stage = newStage;
        isBossSpawned = false;

        spawnInterval = baseSpawnInterval;
        difficultyTimer = 0;

        if(spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if(stage == StageController.Instance.maxStage - 1)
        {
            return;
        }

        spawnCoroutine = StartCoroutine(CoSpawnCat());
    }

    private void HandleBossSpawned()
    {
        isBossSpawned = true;
    }

    IEnumerator CoSpawnCat()
    {
        yield return new WaitForSeconds(spawnInterval);

        while(true)
        {
            if(!isBossSpawned) SpawnRandomCat();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRandomCat()
    {
        int clampedStage = Mathf.Clamp(stage, 0, prefabsCat.Length-1);
        int index = Random.Range(0, clampedStage + 1);

        if (prefabsCat[index] == null) return;

        var obj = Instantiate(prefabsCat[index]);

        float x = Random.Range(-4, 4);
        float y = Random.Range(-4, 4);
        
        obj.transform.position = new Vector3(x, y, 0f);
        obj.target = target;
    }
}
