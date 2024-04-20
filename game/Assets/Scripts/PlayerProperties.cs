using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerProperties : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int Diamonds { get; private set; }
    private bool _isInvincible;

    public UnityEvent<PlayerProperties> onDiamondCollected;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("YOU DEAD!");
        }
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

    public IEnumerator InvincibilityEffect(float duration)
    {
        _isInvincible = true;
        yield return new WaitForSeconds(duration);
        _isInvincible = false;
    }

    public IEnumerator SuperDamageEffect(float duration)
    {
        var gun = GetComponentInChildren<Gun>();
        gun.damage *= 5;
        yield return new WaitForSeconds(duration);
        gun.damage /= 5;
    }

    public IEnumerator SuperFireRateEffect(float duration)
    {
        var gun = GetComponentInChildren<Gun>();
        gun.fireRate *= 5;
        yield return new WaitForSeconds(duration);
        gun.fireRate /= 5;
    }
}