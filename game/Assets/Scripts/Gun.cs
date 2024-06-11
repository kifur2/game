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
    public ParticleSystem defaultImpact;

    public Text ammoDisplay;
    public string shootAudioClipName;
    public string reloadAudioClipName;

    private float _nextTimeToFire = 0f;
    public static bool IsReloading = false;
    public static Coroutine ReloadCoroutine;
    public LayerMask ignoreLayers;
    private float nextTimeToFire = 0;

    private void Update()
    {
        if (Input.GetButton("Fire1") && ammo > 0 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        ammoDisplay.text = ammo + " | " + totalAmmo;
    }

    public void Reload()
    {
        if (IsReloading || ammo >= maxAmmo || PauseManager.IsPaused || totalAmmo <= 0) return;

        IsReloading = true;
        ReloadCoroutine = StartCoroutine(ReloadInternal());
    }

    private IEnumerator ReloadInternal()
    {
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySfxAtPosition(reloadAudioClipName, gameObject.transform.position, 1f);
        }

        yield return new WaitForSeconds(reloadTime);

        if (totalAmmo >= maxAmmo)
        {
            totalAmmo -= (maxAmmo - ammo);
            ammo = maxAmmo;
        }
        else
        {
            ammo = totalAmmo;
            totalAmmo = 0;
        }

        IsReloading = false;
    }

    public void Shoot()
    {
        if (PauseManager.IsPaused || ammo <= 0 || !(Time.time >= _nextTimeToFire)) return;

        _nextTimeToFire = Time.time + 1f / (fireRate * PlayerProperties.FireRateMultiplier);

        muzzleFlash.Play();
        ammo--;

        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySfxAtPosition(shootAudioClipName, gameObject.transform.position, 1f);
        }

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out var hit, range, ~ignoreLayers))
        {
            var target = hit.transform.GetComponent<MonoBehaviour>();

            switch (target)
            {
                case Target t:
                    t.TakeDamage(damage * PlayerProperties.DamageMultiplier);
                    var part = Instantiate(t.impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
                    part.transform.parent = t.transform;
                    break;

                case null:
                    Instantiate(defaultImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    break;
            }
        }
    }
}