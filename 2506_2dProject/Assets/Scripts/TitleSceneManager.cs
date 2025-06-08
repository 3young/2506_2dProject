using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
