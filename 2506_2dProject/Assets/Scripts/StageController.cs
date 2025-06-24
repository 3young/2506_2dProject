using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public static StageController Instance { get; private set; }

    public GameObject finalBossSpawnBtn;

    public int currentStage = 0;
    public int maxStage = 5;

    public int catsAffected = 0;
    public int catsNeedToAffected;

    public UnityEvent<int> OnStageChanged;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [ContextMenu(nameof(OnBossAffected))]
    public void OnBossAffected()
    {
        int catCount = GameManager.Instance.totalCatAffected;
        float clearTime = StageTimerManager.Instance?.currentStageTime ?? 0f;
        int score = ScoreCalculator.Calculate(catCount, clearTime);
        int timeBonus = ScoreCalculator.GetTimeBonus(clearTime);

        bool isFinal = (currentStage >= maxStage - 1);

        UIManager.Instance.StageResultUI.Show(currentStage + 1, catCount, clearTime, score, timeBonus, isFinal);
    }
    public void ProceedToNextStage()
    {
        currentStage++;

        if (currentStage >= maxStage)
        {
            GameManager.Instance.WinGame();
            return;
        }

        catsAffected = 0;
        OnStageChanged?.Invoke(currentStage);
        string nextSceneName = $"Stage{currentStage+1}";
        SceneManager.LoadScene(nextSceneName);
    }

    public void StartStage()
    {
        ArrowPool.Instance.ClearAllArrows();

        if (currentStage < maxStage - 1)
        {
            catsAffected = 0;
            OnStageChanged?.Invoke(currentStage);
            StageTimerManager.Instance?.StartTimer();
        }

        else if(currentStage == maxStage - 1)
        {
            finalBossSpawnBtn?.SetActive(true);
            catsAffected = 0;
            StageTimerManager.Instance?.PauseTimer();
            OnStageChanged?.Invoke(currentStage);
        }
    }

    public void ResetStage()
    {
        currentStage = 0;
        catsAffected = 0;
        OnStageChanged?.Invoke(currentStage);
    }

    public void OnSpawnBossButtonClicked()
    {
        if(finalBossSpawnBtn != null)
        {
            finalBossSpawnBtn.SetActive(false);
        }

        GameManager.Instance?.SpawnFinalBossWithTimeline();
        StageTimerManager.Instance?.StartTimer();
    }
}
