using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageTimerManager : MonoBehaviour
{
    public static StageTimerManager Instance { get; private set; }

    [SerializeField] BossSpawner bossSpawner;

    public float currentStageTime = 0f;
    public UnityEvent<float> OnTimeUpdated;
    public UnityEvent OnTimeReset;
    public UnityEvent OnBossTimeReached;

    private float bossTriggerTime = 10f;
    private bool bossEventFired = false;
    private bool isRunning = true;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        if (StageController.Instance != null)
        {
            StageController.Instance.OnStageChanged.AddListener(ResetTimer);
        }
    }

    private void OnDisable()
    {
        if (StageController.Instance != null)
        {
            StageController.Instance.OnStageChanged.RemoveListener(ResetTimer);
        }
    }

    private void Update()
    {
        if (!isRunning || Time.timeScale == 0f) return;

        currentStageTime += Time.deltaTime;
        OnTimeUpdated?.Invoke(currentStageTime);

        if(!bossEventFired && currentStageTime >= bossTriggerTime)
        {
            OnBossTimeReached?.Invoke();
            bossEventFired = true;
        }
    }

    public void ResetTimer(int newStage)
    {
        currentStageTime = 0f;
        bossEventFired = false;
        OnTimeReset?.Invoke();
    }

    public void PauseTimer() => isRunning = false;
    public void ResumeTimer() => isRunning = true;
}
