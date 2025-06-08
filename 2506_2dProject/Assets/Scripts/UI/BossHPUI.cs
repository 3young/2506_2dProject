using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHPUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtBossName;
    [SerializeField] Image imgBossHp;

    float maxHp;

    public void Setup(string bossName, float maxHp)
    {
        txtBossName.text = bossName;
        this.maxHp = maxHp;
        imgBossHp.fillAmount = 1;
        gameObject.SetActive(true);
    }

    public void UpdateHP(float currentHp)
    {
        Debug.Log($"[BossHPUI] UpdateHP called: current = {currentHp}, max = {maxHp}");
        if (maxHp <= 0)
        {
            Debug.LogWarning("BossHPUI: maxHp is 0 or less! Cannot calculate fillAmount.");
            return;
        }

        imgBossHp.fillAmount = currentHp / maxHp;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
