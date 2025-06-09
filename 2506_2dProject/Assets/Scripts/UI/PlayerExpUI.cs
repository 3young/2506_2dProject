using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpUI : MonoBehaviour
{
    [SerializeField] Image fillExpImage;
    [SerializeField] TMPro.TextMeshProUGUI txtLevel;

    void Start()
    {
        var stats = GameManager.Instance.CurrentPlayer?.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.OnExpChanged.AddListener(UpdateExpUI);
            stats.OnLevelUp.AddListener(UpdateLevelUI);
        }
    }

    private void UpdateExpUI(float current, float max)
    {
        fillExpImage.fillAmount = current / max;
    }

    private void UpdateLevelUI(int level)
    {
        txtLevel.text = $"Lv.{level}";
    }

    public void Connect(PlayerStats stats)
    {
        stats.OnExpChanged.RemoveListener(UpdateExpUI);
        stats.OnLevelUp.RemoveListener(UpdateLevelUI);

        stats.OnExpChanged.AddListener(UpdateExpUI);
        stats.OnLevelUp.AddListener(UpdateLevelUI);

        UpdateLevelUI(stats.Level);
        UpdateExpUI(stats.CurrentExp, stats.MaxExp);
    }
}
