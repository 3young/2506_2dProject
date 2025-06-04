using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI txtTimer;
    
    public void UpdateTimerText(float t)
    {
        int minutes = (int)(t / 60);
        int seconds = (int)(t % 60);
        txtTimer.text = $"{minutes:00}:{seconds:00}";
    }
}
