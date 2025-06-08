using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Xml.Serialization;

public class ResultSceneManager : MonoBehaviour
{
    [Header("입력")]
    [SerializeField] GameObject inputPanel;
    [SerializeField] TMP_InputField inputNameField;
    [SerializeField] Button registerButton;

    [Header("랭킹 표시")]
    [SerializeField] GameObject rankingPanel;
    [SerializeField] Transform rankingContent;
    [SerializeField] GameObject rankingEntryPrefab;
    [SerializeField] Button retryButton;

    private int finalScore;

    private void Start()
    {
        Time.timeScale = 1f;

        retryButton.onClick.AddListener(OnReturnToTitle);

        finalScore = ScoreCalculator.Calculate(GameManager.Instance.totalCatAffected, GameManager.Instance.finalBossClearTime);

        registerButton.onClick.AddListener(OnRegisterName);

        inputPanel.SetActive(true);
        rankingPanel.SetActive(false);
    }

    private void OnRegisterName()
    {
        string playerName = inputNameField.text.Trim();

        if(string.IsNullOrEmpty(playerName)) return;

        RankingManager.Instance.Add(playerName, finalScore);
        inputPanel.SetActive(false);
        ShowRanking();
    }

    private void ShowRanking()
    {
        rankingPanel.SetActive(true);

        var rankings = RankingManager.Instance.GetTop();
        for(int i = 0; i < rankings.Count; i++)
        {
            var entry = rankings[i];
            GameObject gameObject = Instantiate(rankingEntryPrefab, rankingContent);
            var txt = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            txt.text = $"{i + 1}.{entry.name} : {entry.score}";
        }
    }

    public void OnReturnToTitle()
    {
        if (StageController.Instance != null)
        {
            StageController.Instance.ResetStage();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetSavedStats(); 
        }

        SceneManager.LoadScene("TitleScene");
    }
}
