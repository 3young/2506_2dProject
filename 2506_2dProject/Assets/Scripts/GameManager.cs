using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Player prefabPlayer;
    [SerializeField] ArrowPool arrowPool;
    [SerializeField] BossSpawner prefabBossSpawner;
    [SerializeField] CatSpawner prefabCatSpawner;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] StageController prefabStageController;

    private StageController stageController;

    private BossSpawner bossSpawner;
    private CatSpawner catSpawner;

    public int totalCatAffected = 0;
    public bool isGameOver = false;
    private bool isPaused = false;
    public Player CurrentPlayer { get; private set; }

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

        if(StageController.Instance == null)
        {
            stageController = Instantiate(prefabStageController);
            DontDestroyOnLoad (stageController.gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InstantiateSpawners();
        SpawnAndSetupPlayer();

        if (StageController.Instance != null && catSpawner != null)
        {
            StageController.Instance.OnStageChanged.RemoveListener(catSpawner.SetStage);
            StageController.Instance.OnStageChanged.AddListener(catSpawner.SetStage);

            catSpawner.SetStage(StageController.Instance.stage);
        }

        if (virtualCamera != null && CurrentPlayer != null)
        {
            virtualCamera.Follow = CurrentPlayer.transform;
        }

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

        if (health != null)
        {
            health.healthBar = UIManager.Instance?.hpImage;
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

        SceneManager.sceneLoaded += OnSceneLoadedAfterGameOver;   
        StageController.Instance?.ResetStage();
        SceneManager.LoadScene(0);
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
}
