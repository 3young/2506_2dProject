using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Player prefabPlayer;
    [SerializeField] ArrowPool arrowPool;

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
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnAndSetupPlayer();
    }

    private void SpawnAndSetupPlayer()
    {
        if (CurrentPlayer != null) Destroy(CurrentPlayer.gameObject);

        var spawnPos = Vector3.zero;
        CurrentPlayer = Instantiate(prefabPlayer, spawnPos, Quaternion.identity);

        CurrentPlayer.arrowPool = arrowPool;
        CurrentPlayer.healthBar = UIManager.Instance?.hpImage;

        if (catSpawner != null) catSpawner.target = CurrentPlayer.transform;
        if (bossSpawner != null) bossSpawner.target = CurrentPlayer.transform;
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
