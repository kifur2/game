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
        Bow, 
        Ammo
    }

    [SerializeField] private float timedEffectDuration = 0;
    [SerializeField] private Type selectedType;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        var playerProperties = other.GetComponent<PlayerProperties>();
        if (playerProperties == null) return;
        
        var uiIconsQueue = FindObjectOfType<UIIconsQueue>();
        
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
                uiIconsQueue.AddIcon(selectedType, timedEffectDuration);
                break;
            case Type.Sword:
                StartCoroutine(playerProperties.SuperDamageEffect(timedEffectDuration));
                uiIconsQueue.AddIcon(selectedType, timedEffectDuration);
                break;
            case Type.Bow:
                StartCoroutine(playerProperties.SuperFireRateEffect(timedEffectDuration));
                uiIconsQueue.AddIcon(selectedType, timedEffectDuration);
                break;
            case Type.Ammo:
                playerProperties.PickUpAmmo(30);
                break;
            default:
                Debug.LogWarning("Unhandled pickup type: " + selectedType);
                break;
        }

        Destroy(gameObject);
    }
}