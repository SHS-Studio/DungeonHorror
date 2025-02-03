using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; set; }

    public AudioSource ShootingSound;
    public AudioSource ReloadingSound;
    public AudioSource Pistolemptymagsound;
    public AudioSource Attack;

    public AudioClip PistolShot;
    public AudioClip Pistolreload;

    public AudioClip RifelShot;
    public AudioClip Rifellreload;

    public AudioClip EnemyScream;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon) 
        {
            case WeaponModel.Pistol:
                ShootingSound.PlayOneShot(PistolShot);
                break;
            case  WeaponModel.Rifel:
                ShootingSound.PlayOneShot(RifelShot); 
                break;
        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol:
                ReloadingSound.PlayOneShot(Pistolreload);
                break;
            case WeaponModel.Rifel:
                ReloadingSound.PlayOneShot(Rifellreload);
                break;
        }
    }

    public void PlayScream()
    {
        Attack.PlayOneShot(EnemyScream);

    }
}
