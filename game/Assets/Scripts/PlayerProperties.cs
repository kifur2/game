using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerProperties : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int Diamonds { get; private set; }

    [FormerlySerializedAs("OnDiamondCollected")]
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
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("YOU DEAD!");
        }
    }

    public void AddDiamond()
    {
        Diamonds++;
        onDiamondCollected.Invoke(this);
    }
}