using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthScript : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int totalHealth;

    [Header("Callbacks")]
    [SerializeField] private UnityEvent onDamagedEvent;
    [SerializeField] private UnityEvent onDeathEvent;

    private int currentHealth = 0;

    private void OnEnable()
    {
        ResetHealth();
    }

    public void TakeDamage(int dmg)
    {
        UpdateHealth(currentHealth - dmg);
        if(IsAlive())
        {
            onDamagedEvent?.Invoke();
        }
        else
        {
            onDeathEvent?.Invoke();
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    private void ResetHealth()
    {
        currentHealth = totalHealth;
    }

    private void UpdateHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, totalHealth);
    }
}