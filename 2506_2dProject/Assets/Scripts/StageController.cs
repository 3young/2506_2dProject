using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public static StageController Instance { get; private set; }

    public int stage = 1;
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
        stage++;
        OnStageChanged?.Invoke(stage);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if(nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    public void ResetStage()
    {
        stage = 0;
        catsAffected = 0;
        OnStageChanged?.Invoke(stage);
    }
}
