using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CatSpawner : MonoBehaviour
{
    [SerializeField] Cat[] prefabsCat;

    public Transform target;
    public float spawnInterval = 2f;
    public float spawnIntervalDecay = 0.9f;

    private int stage;

    private float difficultyTimer = 0f;
    private float difficultyInterval = 10f;

    private bool isBossSpawned = false;

    private void Start()
    {
        StartCoroutine(nameof(CoSpawnCat));
        if (StageController.Instance != null)
        {
            Debug.Log("[CatSpawner] Registering UpdateStage()");
            StageController.Instance.OnStageChanged.AddListener(UpdateStage);
        }

        if(StageTimerManager.Instance != null)
        {
            StageTimerManager.Instance.OnBossTimeReached.AddListener(HandleBossSpawned);
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

    private void OnDisable()
    {
        if (StageController.Instance != null)
        {
            StageController.Instance.OnStageChanged.RemoveListener(UpdateStage);
        }
    }

    private void UpdateStage(int newStage)
    {
        stage = newStage;
        isBossSpawned = false;
    }

    private void HandleBossSpawned()
    {
        isBossSpawned = true;
    }

    IEnumerator CoSpawnCat()
    {
        while(true)
        {
            if(!isBossSpawned) SpawnRandomCat();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRandomCat()
    {
        int maxIndex = Mathf.Clamp(stage, 0, prefabsCat.Length - 1);
        int index = Random.Range(0, maxIndex + 1);

        var obj = Instantiate(prefabsCat[index]);

        float x = Random.Range(-4, 4);
        float y = Random.Range(-4, 4);
        
        obj.transform.position = new Vector3(x, y, 0f);
        obj.target = target;
    }

    public void SetStage(int newStage)
    {
        stage = newStage;
        isBossSpawned = false;
    }
}
