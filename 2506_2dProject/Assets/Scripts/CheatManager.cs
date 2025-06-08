using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            StageController.Instance?.OnBossAffected();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            var stats = GameManager.Instance.CurrentPlayer?.GetComponent<PlayerStats>();
            stats?.GainExp(stats.MaxExp);
        }
    }
}
