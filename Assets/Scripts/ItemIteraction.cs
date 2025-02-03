using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemIteraction : MonoBehaviour
{
    public static ItemIteraction instance { get; set; }

    public Weapon HoveredWeapon = null;
    public AmmoBox HoveredammoBox = null;
    public BatteryPercentage HoveredBattery = null;
    public PuzzelPiece HoveredPiece = null;

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
        Interaction();
    }

    public void Interaction()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject objecthitbyraycast = hit.transform.gameObject;

            // Select Weapon
            if (objecthitbyraycast.GetComponent<Weapon>() && !objecthitbyraycast.GetComponent<Weapon>().IsactiveWeapon)
            {
                HoveredWeapon = objecthitbyraycast.gameObject.GetComponent<Weapon>();
               // HoveredWeapon.GetComponent<Outline>().enabled = true;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaqponManager.instance.PickUpWeapon(objecthitbyraycast.gameObject);
                }
            }
            else
            {
                if (HoveredWeapon)
                {
                    HoveredammoBox.GetComponent<Outline>().enabled = false;
                }
            }

            // Select Ammo
            if (objecthitbyraycast.GetComponent<AmmoBox>())
            {
                HoveredammoBox = objecthitbyraycast.gameObject.GetComponent<AmmoBox>();
                HoveredammoBox.GetComponent<Outline>().enabled = true;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickUpManager.instance.PickUpAmmo(HoveredammoBox);
                    Destroy(objecthitbyraycast.gameObject);
                    HoveredBattery = GameObject.FindObjectOfType<BatteryPercentage>();
                    Destroy(HoveredBattery.gameObject);
                    AmmoBox[] ammoBox = GameObject.FindObjectsOfType<AmmoBox>();
                    for (int i = 0; i < ammoBox.Length; i++)
                    {
                        Destroy(ammoBox[i].gameObject);
                    }
                  

                }
            }
            else
            {
                if (HoveredammoBox)
                {
                    HoveredammoBox.GetComponent<Outline>().enabled = false;
                }
            }

            // Select Battery

            if (objecthitbyraycast.GetComponent<BatteryPercentage>())
            {
                HoveredBattery = objecthitbyraycast.gameObject.GetComponent<BatteryPercentage>();
                HoveredBattery.GetComponent<Outline>().enabled = true;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickUpManager.instance.PickUpBattery(HoveredBattery);
                    Destroy(objecthitbyraycast.gameObject);
                    AmmoBox[] ammoBox = GameObject.FindObjectsOfType<AmmoBox>();
                    for (int i = 0; i < ammoBox.Length; i++)
                    {
                        Destroy(ammoBox[i].gameObject);
                    }

                }
            }
            else
            {

                if (HoveredBattery)
                {
                    HoveredBattery.GetComponent<Outline>().enabled = false;
                }
            }

            // PickUp Puzzel Pices
            if (objecthitbyraycast.GetComponent<PuzzelPiece>())
            {
                HoveredPiece = objecthitbyraycast.gameObject.GetComponent<PuzzelPiece>();
                HoveredPiece.GetComponent<Outline>().enabled = true;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickUpManager.instance.PickUppuzzelpieces(HoveredPiece);
                    Destroy(objecthitbyraycast.gameObject);
                }
            }
          
        }
    }

  
}
