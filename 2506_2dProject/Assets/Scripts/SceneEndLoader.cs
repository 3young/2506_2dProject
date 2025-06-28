using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneEndLoader : MonoBehaviour
{
    [SerializeField] string bossSceneName = "Stage5";

    public void LoadBossScene(PlayableDirector _)
    {
        SceneManager.LoadSceneAsync(bossSceneName);
    }
}
