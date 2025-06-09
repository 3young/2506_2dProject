using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set;}

    public StageResultUI StageResultUI;

    [SerializeField] private Player player;

    [Header("Player UI")]
    [SerializeField] public Image fillHpImage;
    [SerializeField] TMPro.TextMeshProUGUI txtTimer;
    [SerializeField] Image[] arrowUIImages;
    [SerializeField] public EventSystem eventSystem;

    [Header("Boss UI")]
    [SerializeField] BossHPUI bossHPUI;

    [Header("Ability UI")]
    [SerializeField] public AbilityUIManager AbilityUI;

    public BossHPUI BossHPUI => bossHPUI;
    public Image[] ArrowUIImages => arrowUIImages;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(eventSystem.gameObject);
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
        if (scene.name == "TitleScene")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void ConnectPlayerUI(PlayerStats stats)
    {
        var expUI = GetComponentInChildren<PlayerExpUI>();
        if (expUI != null)
        {
            expUI.Connect(stats);
        }
    }

    public void UpdateTimerText(float t)
    {
        int minutes = (int)(t / 60);
        int seconds = (int)(t % 60);
        txtTimer.text = $"{minutes:00}:{seconds:00}";
    }

    public void ShowStageResult(int stage, int catCount, float clearTime, int score, int timeBonus, bool isFinal)
    {
        StageResultUI.Show(stage, catCount, clearTime, score, timeBonus, isFinal);
    }

    public void LoadFinalRankingScene()
    {
        SceneManager.LoadScene("ResultScene");
    }
}
