using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int totalCatAffected = 0;
    public bool isGameOver = false;
    private bool isPaused = false;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDie += HandlePlayerDied;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDie -= HandlePlayerDied;
    }

    private void HandlePlayerDied()
    {
        if (isGameOver) return;

        isGameOver = true;
        print("Game Over!");

        SceneManager.LoadScene(0);
        totalCatAffected = 0;
        OnResumeGame();
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
}
