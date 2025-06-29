using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public static StageController Instance { get; private set; }

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
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        switch (sceneName)
        {
            case "Stage1":
            case "Stage2":
            case "Stage3":
            case "Stage4":
                AudioManager.Instance.PlayBGM(AudioManager.Instance.stageBgm);
                break;

            case "Stage5":
            case "FinalAnimation":
                AudioManager.Instance.PlayBGM(AudioManager.Instance.bossBgm);
                break;

            case "ResultScene":
                AudioManager.Instance.PlayBGM(AudioManager.Instance.resultBgm);
                break;

            case "TitleScene":
                AudioManager.Instance.PlayBGM(AudioManager.Instance.titleBgm);
                break;
        }
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
        if (currentStage == maxStage - 1)
        {
            SceneManager.LoadScene("FinalAnimation");
        }
        else
        {
            string nextSceneName = $"Stage{currentStage + 1}";
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void StartStage()
    {
        ArrowPool.Instance.ClearAllArrows();

        catsAffected = 0;
        OnStageChanged?.Invoke(currentStage);
        StageTimerManager.Instance?.StartTimer();

    }

    public void ResetStage()
    {
        currentStage = 0;
        catsAffected = 0;
        OnStageChanged?.Invoke(currentStage);
    }
}
