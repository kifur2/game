using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public int selectedWeapon = 0;

    private void Start()
    {
        SelectWeapon(0);
    }

    private void Update()
    {}

    public void SelectWeapon(int selectedWeapon)
    {
        Debug.Log("Select weapon performed for: " + selectedWeapon);
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
}