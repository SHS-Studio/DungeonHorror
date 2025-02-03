using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaqponManager : MonoBehaviour
{
    public static WeaqponManager instance { get; set; }

    [Header("Weapon Slots")]
    public List<GameObject> weaponSlots; // List to hold weapon slots
    public GameObject activeSlot ; // Current active weapon slot
    public bool canSwitchWeapons = false; // Ability to switch weapons unlocked only after an update


    public GameObject bulletimpactprefab;
    public float damage;
    public Weapon m_weapon;

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

    void Start()
    {
        canSwitchWeapons = true;
        activeSlot = weaponSlots[0];
    }

    void Update()
    {
        CheckActiveslots();
        HandleWeaponSwitching();
      


    }


    public void CheckActiveslots()
    {
        foreach (GameObject Slots in weaponSlots)
        {
            if (Slots == activeSlot)
            {
                Slots.SetActive(true);
            }
            else 
            {
                Slots.SetActive(false);
            }
        }

    }

    public void PickUpWeapon(GameObject SelectedWeapon)
    {
        AddweaponinActiveslot(SelectedWeapon);
    }

    private void AddweaponinActiveslot(GameObject PickedWeapon)
    {
        PickedWeapon.transform.SetParent(activeSlot.transform, false);

        Weapon m_weapon =  PickedWeapon.GetComponent<Weapon>();

        PickedWeapon.transform.localPosition = new Vector3(m_weapon.spawnpos.x,m_weapon.spawnpos.y,m_weapon.spawnpos.z);
        PickedWeapon.transform.localRotation = Quaternion.Euler(m_weapon.spawnrot.x,m_weapon.spawnrot.y,m_weapon.spawnrot.z);

        m_weapon.IsactiveWeapon = true;
    }

    public void Switchactiveslot(int slotnumber)
    {

        if (activeSlot.transform.childCount > 0)
        {
            Weapon curntweapon = activeSlot.transform.GetChild(0).GetComponent<Weapon>();
            curntweapon.IsactiveWeapon = false;
        }

        activeSlot =  weaponSlots[slotnumber];

        if (activeSlot.transform.childCount > 0)
        {
            Weapon newweapon = activeSlot.transform.GetChild(0).GetComponent<Weapon>();
            newweapon.IsactiveWeapon = true;
        }
    }

    void HandleWeaponSwitching()
    {
        if (canSwitchWeapons)
        {
            // Scroll wheel input or keyboard input to change weapon slots
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {

                Switchactiveslot(0);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                Switchactiveslot(1);
            }

            // Number key inputs for weapon slots (1 to N)
            for (int i = 0; i < weaponSlots.Count; i++)
            {
                if (Input.GetKeyDown((KeyCode.Alpha1)))
                {
                    Switchactiveslot(0);
                }

                if (Input.GetKeyDown((KeyCode.Alpha2)))
                {
                    Switchactiveslot(1);
                }
            }
        }
    }
     
 
}
