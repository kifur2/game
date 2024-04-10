using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public int ammo;
    public int totalAmmo; 

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;


    public Text ammoDisplay;


    private void Update()
    {
        ammoDisplay.text = ammo.ToString() + " | " + totalAmmo.ToString();

        if (Input.GetButtonDown("Fire1") && ammo > 0) {
            Shoot();
	    }
    }

    void Shoot() {
        RaycastHit hit;
     	muzzleFlash.Play();
        ammo--;
  
	    if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range) ) {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if(target != null) {
                target.TakeDamage(damage);
	        }
	    }
    }
}
