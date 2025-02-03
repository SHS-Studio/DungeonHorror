using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UImanager : MonoBehaviour
{
    public static UImanager instance { get; set; }

    public TextMeshProUGUI ammodisplay;
    public TextMeshProUGUI autoreload;
    public TextMeshProUGUI reload;
    public TextMeshProUGUI MagCount;
    public TextMeshProUGUI Btterychage;

    public Image bullet;
    public Image Mag;
    public Image Flash;

    public Image PistolImg;
    public Image RifelImg;

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

    public void Update()
    {
        WeaponAmmoMagUI();
        Flashlightbatterypecentage();
    }

    public void WeaponAmmoMagUI()
    {
        Weapon Activeweapon = WeaqponManager.instance.activeSlot.GetComponentInChildren<Weapon>();
        Weapon UnActiveweapon = GetunactiveWeaponSlot().GetComponentInChildren<Weapon>();

        if (Activeweapon)
        {
            UImanager.instance.MagCount.text = $"{Activeweapon.bulletsleft / Activeweapon.bulletperbrust}";
            UImanager.instance.ammodisplay.text = $"{PickUpManager.instance.CheckammoleftFor(Activeweapon.thisWeaponModel)}";

            WeaponModel model = Activeweapon.thisWeaponModel;
            bullet.sprite = GetAmmoSprite(model);
            PistolImg.sprite = GetWeaponSprite(model);
            ScaleToFit(PistolImg, 50, 30);
            ScaleToFit(RifelImg, 50, 30);

            Color pistolColor = PistolImg.color;
            pistolColor.a = 1; // Set alpha to 100%
            PistolImg.color = pistolColor;



            if (UnActiveweapon)
            {
                RifelImg.sprite = GetWeaponSprite(UnActiveweapon.thisWeaponModel);
                ScaleToFit(RifelImg, 100, 30);
                ScaleToFit(PistolImg, 100, 30);

                Color UnactiverifelColor = RifelImg.color;
                UnactiverifelColor.a = 0.5f; // Set alpha to 50%
                RifelImg.color = UnactiverifelColor;

            }

        }
        else
        {
            ammodisplay.text = "";
            MagCount.text = "";
        } 
    }

    public Sprite GetWeaponSprite(WeaponModel model)
    {
        switch (model)
        {
            case WeaponModel.Pistol:
                return Instantiate(Resources.Load<GameObject>("Pistol").GetComponent<SpriteRenderer>().sprite);
               
            case WeaponModel.Rifel:
                return Instantiate(Resources.Load<GameObject>("Rifel").GetComponent<SpriteRenderer>().sprite);

                default:
                return null;
        }
    }

    public Sprite GetAmmoSprite(WeaponModel model)
    {
        switch (model)
        {
            case WeaponModel.Pistol:
                return Instantiate(Resources.Load<GameObject>("PistolAmmo").GetComponent<SpriteRenderer>().sprite);
            case WeaponModel.Rifel:
                return Instantiate(Resources.Load<GameObject>("RifelAmmo").GetComponent<SpriteRenderer>().sprite);

            default:
                return null;

        }
    }

    public GameObject GetunactiveWeaponSlot()
    {
        foreach (GameObject Weaponslot in WeaqponManager.instance.weaponSlots)
        {
            if (Weaponslot != WeaqponManager.instance.activeSlot)
            {
                return Weaponslot;
            }
        }
        return null;
    }



    public void Flashlightbatterypecentage()
    {
        
        UImanager.instance.Btterychage.text = $"{PickUpManager.instance.CurntBatteryLevel:00}%";
    }

    void ScaleToFit(Image image, float maxWidth, float maxHeight)
    {
        if (image.sprite == null) return;

        Rect spriteRect = image.sprite.rect;
        float aspectRatio = spriteRect.width / spriteRect.height;

        float width = maxWidth;
        float height = maxWidth / aspectRatio;

        if (height > maxHeight)
        {
            height = maxHeight;
            width = maxHeight * aspectRatio;
        }

        image.rectTransform.sizeDelta = new Vector2(width, height);
    }
}