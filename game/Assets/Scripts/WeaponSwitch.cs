using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public int selectedWeapon = 0;

    void Start()
    {
        SelectWeapon();
    }

    void Update()
    {
        int prevWeapon = selectedWeapon;

        if(Input.GetAxis("Mouse ScrollWheel") > 0f) {
            selectedWeapon++;

            if (selectedWeapon > transform.childCount - 1)
                selectedWeapon = 0;
	    }

        if(Input.GetAxis("Mouse ScrollWheel") < 0f) {
            selectedWeapon--;

            if (selectedWeapon < 0)
                selectedWeapon = transform.childCount - 1;
	    }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedWeapon = 0;

        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
            selectedWeapon = 1;

        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
            selectedWeapon = 2;

        if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
            selectedWeapon = 3;

        if (prevWeapon != selectedWeapon)
            SelectWeapon();
    }

    void SelectWeapon() {
        int i = 0; 
		foreach(Transform weapon in transform) {
            if (i == selectedWeapon) 
                weapon.gameObject.SetActive(true);
            else
	            weapon.gameObject.SetActive(false);
            i++;	
	    }
    }
}
