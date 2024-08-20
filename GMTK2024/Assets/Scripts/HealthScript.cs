using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthScript : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int totalHealth;

    private int currentHealth = 0;

    [Header("Callbacks")]
    [SerializeField] private UnityEvent onDamagedEvent;
    [SerializeField] private UnityEvent onDeathEvent;

    public ScaleMechanicComponent ScalableOwner { get; set; }

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

    public void Kill()
    {
        TakeDamage(currentHealth);
    }

    private void ResetHealth()
    {
        currentHealth = totalHealth;
    }

    private void UpdateHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, totalHealth);
    }

    public void GetValueForInfoDisplay(EntityInfoScript.Info info)
    {
        info.InfoValue = currentHealth.ToString();
    }

    public void SetHealthFromScaling(float newTotalHealth)
    {
        float percent = (float)currentHealth / totalHealth;
        totalHealth = Mathf.FloorToInt(newTotalHealth);
        UpdateHealth(Mathf.FloorToInt((float)totalHealth * percent));
    }

    public void PlaySFX(string key)
    {
        AudioManager.Instance.PlaySFX(key);
    }
}