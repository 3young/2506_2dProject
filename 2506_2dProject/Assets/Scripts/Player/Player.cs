using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerAttack attack;
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerHealth health;

    private AbilitySystem abilitySystem;
    public PlayerAttack Attack => attack;

    private void Awake()
    {
        abilitySystem = GetComponent<AbilitySystem>();
    }

    private void Start()
    {
        stats.LoadFromGameManager();
        stats.OnLevelUp.AddListener(OnPlayerLevelUp);
    }

    private void Update()
    {
        movement.HandleInput();
        attack.HandleAttack();
    }

    private void FixedUpdate()
    {
        movement.Move();
        health.RegenerateHealth();
    }
    private void OnPlayerLevelUp(int newLevel)
    {
        //Debug.Log("플레이어 레벨업 감지됨");
        abilitySystem.OnPlayerLevelUp();
    }

    public void TakeDamage(float amount)
    {
        health.TakeDamage(amount);
    }

    public void Die()
    {
        stats.ResetStats();
        GameManager.Instance.ResetSavedStats();
    }

    public void GainExp(float amount)
    {
        stats.GainExp(amount);
        stats.SaveToGameManager();
    }

    public void GainHp(float amount)
    {
        health.GainHp(amount);
    }

    public PlayerStats Stats => stats;
}
