using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] float baseExp = 20f;
    [SerializeField] int levelExp = 5;

    int playerLevel = 1;
    float currentExp = 0f;
    float maxExp;

    public UnityEvent<float, float> OnExpChanged;
    public UnityEvent<int> OnLevelUp;

    public void LoadFromGameManager()
    {
        playerLevel = GameManager.Instance.savedPlayerLevel;
        currentExp = GameManager.Instance.savedPlayerExp;
        Init();
    }

    private void Init()
    {
        maxExp = baseExp + levelExp * playerLevel;
        OnLevelUp.Invoke(playerLevel);
        OnExpChanged.Invoke(currentExp, maxExp);
    }

    public void GainExp(float amount)
    {
        currentExp += amount;

        while (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            playerLevel++;
            maxExp = baseExp + levelExp * playerLevel;
            OnLevelUp.Invoke(playerLevel);
        }

        OnExpChanged.Invoke(currentExp, maxExp);
    }

    public void ResetStats()
    {
        playerLevel = 1;
        currentExp = 0;
        Init();
    }

    public void SaveToGameManager()
    {
        GameManager.Instance.savedPlayerLevel = playerLevel;
        GameManager.Instance.savedPlayerExp = currentExp;
    }

    public int Level => playerLevel;
    public float CurrentExp => currentExp;
    public float MaxExp => maxExp;
}
