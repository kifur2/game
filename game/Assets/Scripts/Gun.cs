using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Serialization;

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
    public AudioSource audioSource;
    public AudioClip shootAudioClip;
    public AudioClip reloadAudioClip;

    private float _nextTimeToFire = 0f;
    public static bool IsReloading = false;
    public static Coroutine ReloadCoroutine;
    public LayerMask ignoreLayers;

    private void Update()
    {
        ammoDisplay.text = ammo + " | " + totalAmmo;
        if (IsReloading)
            return;

        if (ammo < maxAmmo && Input.GetKeyDown(KeyCode.R))
        {
            Reload();
            return;
        }

        if (!Input.GetButton("Fire1") || ammo <= 0 || !(Time.time >= _nextTimeToFire)) return;
        _nextTimeToFire = Time.time + 1f / (fireRate * PlayerProperties.FireRateMultiplier);
        Shoot();
        ammoDisplay.text = ammo + " | " + totalAmmo;
    }

    private void Reload()
    {
        if (PauseManager.IsPaused || totalAmmo <= 0) return;

        IsReloading = true;
        ReloadCoroutine = StartCoroutine(ReloadInternal());
    }

    private IEnumerator ReloadInternal()
    {
        audioSource.PlayOneShot(reloadAudioClip);
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


    private void Shoot()
    {
        if (PauseManager.IsPaused) return;

        muzzleFlash.Play();
        ammo--;
        if (audioSource != null)
        {
            audioSource.PlayOneShot(shootAudioClip);
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