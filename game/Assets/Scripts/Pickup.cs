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

    private const string BonusAudioClipName = "PickUp";

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
                playerProperties.StartEffect(timedEffectDuration, PlayerProperties.TemporaryEffect.Invincibility);
                uiIconsQueue.AddIcon(selectedType, timedEffectDuration);
                break;
            case Type.Sword:
                playerProperties.StartEffect(timedEffectDuration, PlayerProperties.TemporaryEffect.SuperDamage);
                uiIconsQueue.AddIcon(selectedType, timedEffectDuration);
                break;
            case Type.Bow:
                playerProperties.StartEffect(timedEffectDuration, PlayerProperties.TemporaryEffect.SuperFireRate);
                uiIconsQueue.AddIcon(selectedType, timedEffectDuration);
                break;
            case Type.Ammo:
                playerProperties.PickUpAmmo(30);
                break;
            default:
                Debug.LogWarning("Unhandled pickup type: " + selectedType);
                break;
        }
        
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySfxAtPosition(BonusAudioClipName, transform.position, 1f);
        }
        
        Destroy(gameObject);
    }
}