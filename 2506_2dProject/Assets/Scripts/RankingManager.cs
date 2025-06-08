using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class RankingEntry
{
    public string name;
    public int score;

    public RankingEntry(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance { get; private set; }

    private List<RankingEntry> rankings = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void Add(string name, int score)
    {
        rankings.Add(new RankingEntry(name, score));
        rankings = rankings.OrderByDescending(r => r.score).Take(5).ToList();
    }

    public List<RankingEntry> GetTop() => rankings;
}
