using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIconsQueue : MonoBehaviour
{
    public GameObject invincibilityIconPrefab;
    public GameObject superDamageIconPrefab;
    public GameObject superFireRateIconPrefab;
    public GameObject emptyIconPrefab;

    private readonly Dictionary<Pickup.Type, IconInfo> _icons = new Dictionary<Pickup.Type, IconInfo>();

    private class IconInfo
    {
        public GameObject IconGameObject;
        public Coroutine IconCoroutine;
    }

    public void AddIcon(Pickup.Type iconType, float iconLifetime)
    {
        var iconPrefab = iconType switch
        {
            Pickup.Type.Shield => invincibilityIconPrefab,
            Pickup.Type.Sword => superDamageIconPrefab,
            Pickup.Type.Bow => superFireRateIconPrefab,
            _ => emptyIconPrefab,
        };

        if (_icons.TryGetValue(iconType, out IconInfo existingIcon))
        {
            StopCoroutine(existingIcon.IconCoroutine);
            Destroy(existingIcon.IconGameObject);
            _icons.Remove(iconType);
        }

        var iconGameObject = Instantiate(iconPrefab, gameObject.transform);
        var newIconInfo = new IconInfo
        {
            IconGameObject = iconGameObject,
            IconCoroutine = StartCoroutine(RemoveIconAfterDelay(iconType, iconLifetime))
        };
        _icons[iconType] = newIconInfo;
    }

    private IEnumerator RemoveIconAfterDelay(Pickup.Type iconType, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!_icons.TryGetValue(iconType, out var iconInfo)) yield break;
        Destroy(iconInfo.IconGameObject);
        _icons.Remove(iconType);
    }
}