using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageResultUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtStage;
    [SerializeField] TextMeshProUGUI txtCatCount;
    [SerializeField] TextMeshProUGUI txtTime;
    [SerializeField] TextMeshProUGUI txtTimeBonus;
    [SerializeField] TextMeshProUGUI txtTotalScore;
    [SerializeField] GameObject nextStageButton;

    public void Show(int stageNum, int catCount, float clearTime, int score, int timeBonus, bool isFinal)
    {
        gameObject.SetActive(true);

        txtStage.text = $"STAGE{stageNum} CLEAR";
        txtCatCount.text = $"ATTRACTED CAT : {catCount} x 100 = {catCount * 100}";
        txtTime.text = $"TIME : {clearTime:F2}s";
        txtTimeBonus.text = $"TIME BONUS : {timeBonus}";
        txtTotalScore.text = $"TOTAL SCORE : {score}";

        nextStageButton.SetActive(!isFinal);

        Time.timeScale = 0f;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClickNextStage()
    {
        StageController.Instance.ProceedToNextStage();
        Time.timeScale = 1f;
    }

}
