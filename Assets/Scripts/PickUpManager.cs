using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PickUpManager : MonoBehaviour
{
    public static PickUpManager instance { get; set; }
    [Header("Ammo")]
    public int totalpistolammo;
    public int totalrifelammo;


    [Header("Battery")]
    public float CurntBatteryLevel;
    public float Chargingpercentage;

    [Header("Puzzel")]
    public int puzelpiece;
    public int totalpiececount;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickUpAmmo(AmmoBox ammo)
    {
        switch (ammo.ammotype)
        {
            case AmmoType.Pistol:
                totalpistolammo += ammo.AmmoAmnt;
                break;
            case AmmoType.Rifel:
                totalrifelammo += ammo.AmmoAmnt;
                break;
        }
    }
    public void PickUpBattery(BatteryPercentage battery)
    {
        Chargingpercentage = battery.BatteryChargingPercentage;
        CurntBatteryLevel += Chargingpercentage;
        CurntBatteryLevel = Mathf.Clamp(CurntBatteryLevel, 0f, 100f);
    }

    public void PickUppuzzelpieces(PuzzelPiece pieces)
    {
        pieces = pieces.GetComponent<PuzzelPiece>();
        Puzzelmanager.instance.PickUpPuzzelpieces(pieces.gameObject);
        puzelpiece = pieces.puzzelpieceID; 
        totalpiececount++;

       
    }


    public int CheckammoleftFor(WeaponModel thisweaponmodel)
    {
        switch (thisweaponmodel)
        {
            case WeaponModel.Pistol:

                return totalpistolammo;
            case WeaponModel.Rifel:

                return totalrifelammo;
            default:
                return 0;
        }
    }
    public void DecreaseTotalAmmo(int bullettodecrease, WeaponModel thisweaponmodel)
    {
        switch (thisweaponmodel)
        {
            case WeaponModel.Pistol:
                totalpistolammo -= bullettodecrease;
                break;
            case WeaponModel.Rifel:
                totalrifelammo -= bullettodecrease;
                break;
        }
    }
}
