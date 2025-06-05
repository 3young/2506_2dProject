using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set;}

    [SerializeField] public Image hpImage;
    [SerializeField] TMPro.TextMeshProUGUI txtTimer;
    [SerializeField] Image[] arrowUIImages;

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
    }

    public void UpdateTimerText(float t)
    {
        int minutes = (int)(t / 60);
        int seconds = (int)(t % 60);
        txtTimer.text = $"{minutes:00}:{seconds:00}";
    }
}
