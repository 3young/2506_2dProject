using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerAttack attack;
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerHealth health;

    private void Start()
    {
        stats.LoadFromGameManager();
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

    public void TakeDamage(float amount)
    {
        health.TakeDamage(amount);
    }

    public void Die()
    {
        stats.ResetStats();
        stats.SaveToGameManager();
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
