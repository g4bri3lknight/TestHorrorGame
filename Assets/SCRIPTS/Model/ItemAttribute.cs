using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemAttribute : ScriptableObject
{
    private  string slotName;

    private string itemName;

    private string itemInfo;

    private ItemType itemType;

    private string itemImageName;

    private int ammosQuantity;

    private string weaponTargetName;

    private float healthValue;

    private string targetObjName;


    public string SlotName
    {
        get
        {
            return this.slotName;
        }
        set
        {
            slotName = value;
        }
    }

    public string ItemName
    {
        get
        {
            return this.itemName;
        }
        set
        {
            itemName = value;
        }
    }

    public string ItemInfo
    {
        get
        {
            return this.itemInfo;
        }
        set
        {
            itemInfo = value;
        }
    }

    public ItemType ItemType
    {
        get
        {
            return this.itemType;
        }
        set
        {
            itemType = value;
        }
    }

    public string ItemImageName
    {
        get
        {
            return this.itemImageName;
        }
        set
        {
            itemImageName = value;
        }
    }

    public int AmmosQuantity
    {
        get
        {
            return this.ammosQuantity;
        }
        set
        {
            ammosQuantity = value;
        }
    }

    public string WeaponTargetName
    {
        get
        {
            return this.weaponTargetName;
        }
        set
        {
            weaponTargetName = value;
        }
    }

    public float HealthValue
    {
        get
        {
            return this.healthValue;
        }
        set
        {
            healthValue = value;
        }
    }

    public string TargetObjName
    {
        get
        {
            return this.targetObjName;
        }
        set
        {
            targetObjName = value;
        }
    }
}
