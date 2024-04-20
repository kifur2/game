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

    [FormerlySerializedAs("OnDiamondCollected")] public UnityEvent<PlayerProperties> onDiamondCollected;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        //actions when we take damage
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void AddDiamond()
    {
        Diamonds++;
        onDiamondCollected.Invoke(this);
    }
}