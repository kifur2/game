using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float reloadTime = 5f;

    public int maxAmmo;
    public int ammo;
    public int totalAmmo; 

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;

    public Text ammoDisplay;

    private float _nextTimeToFire = 0f;
    private bool _isReloading = false;

    private void Update()
    {
        ammoDisplay.text = ammo.ToString() + " | " + totalAmmo.ToString();
        if (_isReloading)
            return;

        if(ammo < maxAmmo && Input.GetKeyDown(KeyCode.R)) {
            StartCoroutine(Reload());
            return;
	    }

        if (!Input.GetButton("Fire1") || ammo <= 0 || !(Time.time >= _nextTimeToFire)) return;
        _nextTimeToFire = Time.time + 1f / fireRate;
        Shoot();
        ammoDisplay.text = ammo.ToString() + " | " + totalAmmo.ToString();
    }

    IEnumerator Reload() {
        if(totalAmmo > 0) {
            _isReloading = true;
			yield return new WaitForSeconds(reloadTime);
            if( totalAmmo >= maxAmmo ) { 
				totalAmmo -= (maxAmmo - ammo); 
				ammo = maxAmmo;
	        }  else {
                ammo = totalAmmo;
                totalAmmo = 0;
	        }
            _isReloading = false;
    	}
    }

    private void Shoot() {
        RaycastHit hit;
     	muzzleFlash.Play();
        ammo--;

        if (!Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) return;
        var target = hit.transform.GetComponent<Target>();
        if(target != null) {
            target.TakeDamage(damage);
        }
    }


}
