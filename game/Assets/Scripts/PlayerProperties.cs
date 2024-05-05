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
    public AudioClip deathAudioClip;

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
            AudioSource.PlayClipAtPoint(deathAudioClip, gameObject.transform.position);
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

    public void PickUpAmmo(int ammoNo)
    {
        var gun = GetComponentInChildren<Gun>();
        gun.totalAmmo += ammoNo;
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
        gun.damage *= 2;
        yield return new WaitForSeconds(duration);
        gun.damage /= 2;
    }

    public IEnumerator SuperFireRateEffect(float duration)
    {
        var gun = GetComponentInChildren<Gun>();
        gun.fireRate *= 1.5f;
        yield return new WaitForSeconds(duration);
        gun.fireRate /= 1.5f;
    }

    
} 