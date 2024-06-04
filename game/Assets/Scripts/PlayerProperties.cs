using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerProperties : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int Diamonds { get; private set; }
    private bool _isInvincible;

    public UnityEvent<PlayerProperties> onDiamondCollected;

    public HealthBar healthBar;

    public static float DamageMultiplier = 1f;
    public static float FireRateMultiplier = 1f;
    public CharacterDeathManager characterDeathManager;

    public enum TemporaryEffect
    {
        Invincibility,
        SuperDamage,
        SuperFireRate
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth > 0) return;

        characterDeathManager.TriggerDeath();
    }

    public void Heal(int healAmount = 30)
    {
        currentHealth += healAmount;
        healthBar.SetHealth(currentHealth);
    }

    public void AddDiamond()
    {
        Diamonds++;
        onDiamondCollected.Invoke(this);
    }

    public void PickUpAmmo(int ammoNo)
    {
        var gun = GetComponentInChildren<Gun>();
        gun.totalAmmo += ammoNo;
    }

    public void StartEffect(float duration, TemporaryEffect effectType)
    {
        switch (effectType)
        {
            case TemporaryEffect.Invincibility:
                StartCoroutine(InvincibilityEffect(duration));
                break;
            case TemporaryEffect.SuperDamage:
                StartCoroutine(SuperDamageEffect(duration));
                break;
            case TemporaryEffect.SuperFireRate:
                StartCoroutine(SuperFireRateEffect(duration));
                break;
        }
    }

    private IEnumerator InvincibilityEffect(float duration)
    {
        _isInvincible = true;
        yield return new WaitForSeconds(duration);
        _isInvincible = false;
    }

    private static IEnumerator SuperDamageEffect(float duration)
    {
        DamageMultiplier = 5f;
        yield return new WaitForSeconds(duration);
        DamageMultiplier /= 5f;
    }

    private static IEnumerator SuperFireRateEffect(float duration)
    {
        FireRateMultiplier = 1.5f;
        yield return new WaitForSeconds(duration);
        FireRateMultiplier /= 1.5f;
    }
}