using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] Player prefabPlayer;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    [Header("Spawn")]
    [SerializeField] BossSpawner prefabBossSpawner;
    [SerializeField] CatSpawner prefabCatSpawner;
    [SerializeField] ArrowPool arrowPool;
    [SerializeField] SideHeartPool sideHeartPool;
    [SerializeField] StageController prefabStageController;

    [SerializeField] Button finalBossSpawnBtn;

    private StageController stageController;
    private BossSpawner bossSpawner;
    private CatSpawner catSpawner;
    
    public int totalCatAffected = 0;
    public bool isGameOver = false;
    private bool isPaused = false;

    public int savedPlayerLevel = 1;
    public float savedPlayerExp = 0f;
    public float finalBossClearTime = 0f;
    public int savedSideHeartLevel = 1;

    public Player CurrentPlayer { get; private set; }
    public FinalBoss CurrentBoss { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Destroy(virtualCamera.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(virtualCamera.gameObject);

        SetupSideHeartPool();

        if (StageController.Instance == null)
        {
            stageController = Instantiate(prefabStageController);
            DontDestroyOnLoad (stageController.gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("임시 보스 버튼 강제 호출!");
            StageController.Instance.OnSpawnBossButtonClicked();
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ResultScene") return;

        InstantiateSpawners();
        SpawnAndSetupPlayer();

        if (StageController.Instance != null && catSpawner != null)
        {
            StageController.Instance.OnStageChanged.RemoveListener(catSpawner.UpdateStage);
            StageController.Instance.OnStageChanged.AddListener(catSpawner.UpdateStage);
            StageController.Instance.OnStageChanged?.Invoke(StageController.Instance.currentStage);
        }

        if(finalBossSpawnBtn != null)
        {
            StageController.Instance.finalBossSpawnBtn = finalBossSpawnBtn.gameObject;
            finalBossSpawnBtn.gameObject.SetActive(false);

            finalBossSpawnBtn.onClick.RemoveAllListeners();
            finalBossSpawnBtn.onClick.AddListener(StageController.Instance.OnSpawnBossButtonClicked);
        }

        SetupCamera();
        StageController.Instance?.StartStage();
    }
    private void SetupSideHeartPool()
    {
        if (SideHeartPool.Instance == null)
        {
            Instantiate(sideHeartPool);
        }
    }

    private void SetupCamera()
    {
        if (virtualCamera != null && CurrentPlayer != null)
        {
            virtualCamera.Follow = CurrentPlayer.transform;
            var confinder = virtualCamera.GetComponent<CinemachineConfiner2D>();
            var cameraArea = GameObject.FindWithTag("CameraArea")?.GetComponent<PolygonCollider2D>();

            if (confinder != null && cameraArea != null)
            {
                var collider = cameraArea.GetComponent<Collider2D>();
                confinder.m_BoundingShape2D = collider;
                confinder.InvalidateCache();
            }

            var limiter = CurrentPlayer.GetComponent<PlayerBoundsLimiter>();
            if (limiter != null)
            {
                limiter.SetBounds(cameraArea);
            }
        }
    }

    private void InstantiateSpawners()
    {
        if (catSpawner != null && bossSpawner != null) return;

        if (bossSpawner == null)
        {
            bossSpawner = Instantiate(prefabBossSpawner);
            RegisterBossSpawner(bossSpawner);

            if(StageTimerManager.Instance != null)
            {
                StageTimerManager.Instance.RegisterSpawner(bossSpawner);
            }
        }

        if (catSpawner == null)
        {
            catSpawner = Instantiate(prefabCatSpawner);
            RegisterCatSpawner(catSpawner);
        }
    }
    private void SpawnAndSetupPlayer()
    {

        if (CurrentPlayer != null) Destroy(CurrentPlayer.gameObject);

        var spawnPos = Vector3.zero;
        CurrentPlayer = Instantiate(prefabPlayer, spawnPos, Quaternion.identity);

        var stats = CurrentPlayer.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.LoadFromGameManager();
            UIManager.Instance.ConnectPlayerUI(stats); 
        }

        var attack = CurrentPlayer.GetComponent<PlayerAttack>();
        if (attack != null)
        {
            attack.arrowPool = arrowPool;
            attack.arrowCounts = UIManager.Instance.ArrowUIImages;
        }

        var health = CurrentPlayer.GetComponent<PlayerHealth>();

        if (virtualCamera != null)
        {
            virtualCamera.Follow = CurrentPlayer.transform;
        }

        if (health != null && UIManager.Instance != null)
        {
            health.healthBar = UIManager.Instance?.fillHpImage;
            health.ResetHealth();
        }

        if (catSpawner != null) catSpawner.target = CurrentPlayer.transform;
        if (bossSpawner != null) bossSpawner.target = CurrentPlayer.transform;

        isGameOver = false;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDie += HandlePlayerDied;
        GameEvents.OnBossCaptivated += HandleBossCaptivated;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDie -= HandlePlayerDied;
        GameEvents.OnBossCaptivated -= HandleBossCaptivated;
    }

    private void HandlePlayerDied()
    {
        if (isGameOver) return;

        isGameOver = true;
        print("Game Over!");

        totalCatAffected = 0;
        ResetSavedStats();
        UIManager.Instance.BossHPUI.Hide();

        SceneManager.sceneLoaded += OnSceneLoadedAfterGameOver;   
        StageController.Instance?.ResetStage();
        SceneManager.LoadScene(1);
    }

    private void OnSceneLoadedAfterGameOver(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedAfterGameOver;

        if (StageTimerManager.Instance != null) StageTimerManager.Instance.ResetTimer(0);
    }

    private void HandleBossCaptivated()
    {
        StageController.Instance.OnBossAffected();
    }

    public void SaveStatsFromPlayer(PlayerStats stats)
    {
        savedPlayerLevel = stats.Level;
        savedPlayerExp = stats.CurrentExp;
    }

    public void ResetSavedStats()
    {
        savedPlayerLevel = 1;
        savedPlayerExp = 0f;
    }

    public void OnPauseGame()
    {
        Time.timeScale = 0;
        StageTimerManager.Instance.PauseTimer();
        isPaused = true;
    }

    public void OnResumeGame()
    {
        Time.timeScale = 1;
        StageTimerManager.Instance.ResumeTimer();
        isPaused = false;
    }

    public void TogglePause()
    {
        if (isPaused) OnResumeGame();
        else OnPauseGame();
    }

    public void RegisterBossSpawner(BossSpawner spawner)
    {
        bossSpawner = spawner;
    }

    public void RegisterCatSpawner(CatSpawner spawner)
    {
        catSpawner = spawner;
    }

    public void RegisterBoss(FinalBoss boss)
    {
        CurrentBoss = boss;
    }

    public void ForceSpawnFinalBoss()
    {
        if(bossSpawner != null)
        {
            bossSpawner.SpawnBoss();
        }
    }

    public void WinGame()
    {
        if (isGameOver) return;

        isGameOver = true;

        if(CurrentPlayer != null)
        {
            var stats = CurrentPlayer.GetComponent<PlayerStats>();
            if (stats != null)
            {
                SaveStatsFromPlayer(stats);
            }
        }

        finalBossClearTime = StageTimerManager.Instance?.currentStageTime ?? 0f;

        int catCount = totalCatAffected;
        float time = finalBossClearTime;
        int score = ScoreCalculator.Calculate(catCount, time);

        SceneManager.LoadScene("ResultScene");
    } 

}
