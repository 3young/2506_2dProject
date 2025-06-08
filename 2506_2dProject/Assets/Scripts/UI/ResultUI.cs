using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtLevel;
    [SerializeField] TextMeshProUGUI txtExp;
    [SerializeField] TextMeshProUGUI txtScore;
    [SerializeField] TextMeshProUGUI txtClearTime;

    private void Start()
    {
        int level = GameManager.Instance.savedPlayerLevel;
        float exp = GameManager.Instance.savedPlayerExp;
        float time = GameManager.Instance.finalBossClearTime;

        int score = CalculateScore(level, exp);

        txtLevel.text = $"Lv.{level:00}";
        txtExp.text = $"EXP : {exp:0}";
        txtScore.text = $"Score : {score}";
        txtClearTime.text = $"Time : {FormatTime(time)}";
    }

    int CalculateScore(int level, float exp)
    {
        return level * 1000 + Mathf.RoundToInt(exp * 10f);
    }

    string FormatTime(float seconds)
    {
        int min = (int)(seconds / 60f);
        int sec = (int)(seconds % 60f);
        return $"{min:00}:{sec:00}";
    }
}
