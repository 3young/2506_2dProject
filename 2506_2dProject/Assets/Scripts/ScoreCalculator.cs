
using UnityEngine;

public static class ScoreCalculator
{
    public static int Calculate(int catCount, float clearTime, float timeLimit = 100f)
    {
        int catScore = catCount * 100;
        int timeBonus = clearTime <= timeLimit
            ? (int)(timeLimit - clearTime) * 10
            : -(int)(clearTime - timeLimit) * 5;

        return Mathf.Max(0, catScore + timeBonus);
    }

    public static int GetTimeBonus(float clearTime, float timeLimit = 100f)
    {
        return clearTime <= timeLimit
            ? (int)(timeLimit - clearTime) * 10
            : -(int)(clearTime - timeLimit) * 5;
    }

}
