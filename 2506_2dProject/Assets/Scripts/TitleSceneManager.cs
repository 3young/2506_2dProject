using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.titleBgm);
    }
    public void OnClickStart()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
