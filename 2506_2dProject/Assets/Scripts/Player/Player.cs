using System.Collections;   
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerAttack attack;
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerHealth health;
    
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
}
