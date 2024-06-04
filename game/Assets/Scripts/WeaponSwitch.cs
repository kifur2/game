using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public int selectedWeapon = 0;
    private readonly int _weaponsAmount = 3;

    private void Start()
    {
        SelectWeapon(0);
    }

    public void SelectWeapon(int selectedWeapon)
    {
        this.selectedWeapon = selectedWeapon;

        if (Gun.ReloadCoroutine != null)
        {
            StopCoroutine(Gun.ReloadCoroutine);
            Gun.ReloadCoroutine = null;
            Gun.IsReloading = false;
        }

        var i = 0;
        foreach (Transform weapon in transform)
        {
            weapon.gameObject.SetActive(i == selectedWeapon);
            i++;
        }
    }

    public void SwitchWeapon(float x)
    {
        switch (x)
        {
            case > 0:
                SelectWeapon((selectedWeapon + 1) % _weaponsAmount);
                break;
            case < 0:
                SelectWeapon((selectedWeapon + _weaponsAmount - 1) % _weaponsAmount);
                break;
        }
    }
}