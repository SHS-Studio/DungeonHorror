using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum WeaponType
{
    Melee,
    Ranged,
    Magic
}
public enum WeaponModel
{
    Pistol,
    Rifel,
    SMG,
    HMG
}
public enum ShootingMode
{
    Single,
    Btrust,
    Auto
}

public class Weapon : MonoBehaviour
{
    [Header("General Settings")]
    public WeaponType CurntweaponType;
    public WeaponModel thisWeaponModel;
    public ShootingMode CurntshotingMode;
    public Transform attackPoint; // Point of attack origin (used for ranged/magic)
    public Animator m_animator;
    public GameObject Muzzeleffect;
    public float damage = 10f;

    [Header("Shooting")]
    public bool Isshooting;
    public bool readytoShoot;
    public float shootingDelay;// Attacks per second
    bool allowreset = true;

    [Header("Reloading")]
    public float reloadtime;
    public float autoreloadWaittime;
    public float autoReloadPenaltyTime = 2f; // Penalty time before auto-reload starts
    public int magazinesize, bulletsleft;
    public bool Isreloading;

    [Header("Melee Settings")]
    public float meleeRange = 2f;
    public LayerMask enemyLayers; // Layers considered as enemies

    [Header("Ranged Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public float projectilelifetime;
    // Weapom Spawn Rotatin & Position
    public Vector3 spawnpos;
    public Vector3 spawnrot;
    // Brust
    public int bulletperbrust = 3;
    public int brustbulletleft;
    // Spread
    public float spreadintensity;
  

    [Header("Magic Settings")]
    public GameObject magicEffectPrefab;

    [Header("UI")]
    public TextMeshProUGUI ammodisplay;
    public TextMeshProUGUI autoReloadMessageDisplay; // UI element for auto-reload message

    [Header("ActiveWeapon")]
    public bool IsactiveWeapon = false;
    public void Awake()
    {
        readytoShoot = true;
        brustbulletleft = bulletperbrust;
        m_animator = GetComponent<Animator>();
        bulletsleft = magazinesize;
    }

    void Update()
    {
       
        if (IsactiveWeapon)
        {
            Attack();
            Reload();
            Autoreload();
            Emptyshot();
        }  
    }

    void Attack()
    {
        switch (CurntweaponType)
        {
            case WeaponType.Melee:
                PerformMeleeAttack();
                break;

            case WeaponType.Ranged:
                FireRangeWeapon();
                break;

            case WeaponType.Magic:
                PerformMagicAttack();
                break;
        }
    }

    void FireRangeWeapon()
    {
        if (CurntshotingMode == ShootingMode.Auto)
        {
            Isshooting = Input.GetMouseButton(0);
        }
        else if (CurntshotingMode == ShootingMode.Single || CurntshotingMode == ShootingMode.Btrust)
        {
            Isshooting = Input.GetMouseButtonDown(0);
        }

        if (readytoShoot && Isshooting && !Isreloading && bulletsleft > 0)
        {
            brustbulletleft = bulletperbrust;
            PerformRangedAttack();
        }
    }

    void PerformMeleeAttack()
    {
        Debug.Log("Performing melee attack.");
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, meleeRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log($"Hit {enemy.name}");
            // Assume the enemy has a script with a TakeDamage method
            // enemy.GetComponent<Enemy>()?.TakeDamage(damage);
        }
    }

    void PerformRangedAttack()
    {
        bulletsleft--;
        Muzzeleffect.GetComponent<ParticleSystem>().Play();
        m_animator.SetTrigger("Recoil");
        //SoundManager.instance.Pistolfiringsound.Play();
        SoundManager.instance.PlayShootingSound(thisWeaponModel);
        readytoShoot = false;

        Vector3 shootingdirection = CalculateDirectionAndSpread().normalized;
        Debug.Log("Performing ranged attack.");
        if (projectilePrefab != null)
        {
            GameObject projectileClone = Instantiate(projectilePrefab, attackPoint.position, attackPoint.rotation);
            projectileClone.transform.forward = shootingdirection;
            Debug.Log("D" + shootingdirection);
            Rigidbody rb = projectileClone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(shootingdirection * projectileSpeed, ForceMode.Impulse);
                StartCoroutine(destroybulletaftertime(projectileClone, projectilelifetime));
            }
        }

        if (allowreset)
        {
            Invoke("ResetShot", shootingDelay);
            allowreset = false;
        }

        if (CurntshotingMode == ShootingMode.Btrust && brustbulletleft > 1)
        {
            brustbulletleft--;
            Invoke("FireRangeWeapon", shootingDelay);
        }
    }

    void PerformMagicAttack()
    {
        Debug.Log("Performing magic attack.");
        if (magicEffectPrefab != null)
        {
            Instantiate(magicEffectPrefab, attackPoint.position, attackPoint.rotation);
        }
    }

    public IEnumerator destroybulletaftertime(GameObject Bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(Bullet);
    }

    public void Reload()
    {
        if (Input.GetKey(KeyCode.R) && bulletsleft < magazinesize && !Isreloading && PickUpManager.instance.CheckammoleftFor(thisWeaponModel) > 0 )
        {
            m_animator.SetTrigger("Reload");
           // SoundManager.instance.Pistolreloadsound.Play();
            SoundManager.instance.PlayReloadSound(thisWeaponModel);
            Isreloading = true;
            Invoke("ReloadComplete", reloadtime);
        }
    }

    public void Autoreload()
    {
        // Start auto-reload coroutine if not already running
        if (bulletsleft <= 0 && !Isreloading && PickUpManager.instance.CheckammoleftFor(thisWeaponModel) > 0)
        {
            StartCoroutine(AutoreloadWithPenalty());
        }
    }

    private IEnumerator AutoreloadWithPenalty()
    {
        Isreloading = true;

        if (UImanager.instance.autoreload.text != null)
        {
            // Countdown before auto-reload starts
            for (int i = (int)autoReloadPenaltyTime; i > 0; i--)
            {
                UImanager.instance.autoreload.text = $"Auto reload starts in {i} seconds";
                yield return new WaitForSeconds(autoreloadWaittime);
            }
        }

        // Display reloading message and play the reload sound once
        if (UImanager.instance.autoreload!= null)
        {
            UImanager.instance.autoreload.text = "Reloading...";
        }
        m_animator.SetTrigger("Reload");
        //SoundManager.instance.Pistolreloadsound.Play(); // Play reload sound here
        SoundManager.instance.PlayReloadSound(thisWeaponModel);
        // Wait for the reload duration
        yield return new WaitForSeconds(reloadtime);

        // Complete reload
        ReloadComplete();

        // Clear the message
        if (UImanager.instance.autoreload != null)
        {
            UImanager.instance.autoreload.text = "";
        }
    }

    public void ReloadComplete()
    {
        if (PickUpManager.instance.CheckammoleftFor(thisWeaponModel) > magazinesize)
        {
            bulletsleft = magazinesize;
            PickUpManager.instance.DecreaseTotalAmmo(bulletsleft,thisWeaponModel);
        }
        else
        {
            bulletsleft = PickUpManager.instance.CheckammoleftFor(thisWeaponModel);
            PickUpManager.instance.DecreaseTotalAmmo(bulletsleft, thisWeaponModel);

        }
       
        Isreloading = false;
    }

    public void ResetShot()
    {
        readytoShoot = true;
        allowreset = true;
    }

    public void Emptyshot()
    {
        if (bulletsleft == 0 && Isshooting)
        {
            SoundManager.instance.Pistolemptymagsound.Play();
        }
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetpoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetpoint = hit.point;
        }
        else
        {
            targetpoint = ray.GetPoint(100);
        }

        Vector3 direction = targetpoint - attackPoint.position;

        float x = UnityEngine.Random.Range(-spreadintensity, spreadintensity);
        float y = UnityEngine.Random.Range(-spreadintensity, spreadintensity);
        return direction + new Vector3(x, y, 0);
    }

   

    void OnDrawGizmosSelected()
    {
        if (CurntweaponType == WeaponType.Melee && attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, meleeRange);
        }
    }
}