using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum Type
    {
        Diamond,
        Heart,
        Shield,
        Sword,
        Cartridge
    }

    [SerializeField] private float timedEffectDuration = 0;
    [SerializeField] private Type selectedType;

    private void OnTriggerEnter(Collider other)
    {
        var playerProperties = other.GetComponent<PlayerProperties>();
        if (playerProperties == null || !other.CompareTag("Player")) return;
        switch (selectedType)
        {
            case Type.Diamond:
                playerProperties.AddDiamond();
                break;
            case Type.Heart:
                playerProperties.Heal();
                break;
            case Type.Shield:
                StartCoroutine(playerProperties.InvincibilityEffect(timedEffectDuration));
                break;
            case Type.Sword:
                StartCoroutine(playerProperties.SuperDamageEffect(timedEffectDuration));
                break;
            case Type.Cartridge:
                StartCoroutine(playerProperties.SuperFireRateEffect(timedEffectDuration));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Destroy(gameObject);
    }
}