//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using UnityEngine;

public class HDK_InventoryItem : MonoBehaviour {

    [Header("Main Item Settings")]
    public string itemName;
    public string itemInfo;
    public ItemType itemType;
    public Sprite itemIcon;
    public bool ObjectKnown;

    [Header("Examination Settings")]
    public float Distance;
    Vector3 startPos;
    Quaternion startRot;
    Vector3 endPos;

    [Header("Ammos Settings")]
    public int AmmosQuantity;

    [Header("Weapon Settings")]
    public GameObject WeaponTarget;

    [Header("Eatable Settings")]
    public float HealthValue;

    [Header("Key Settings")]
    public GameObject TargetObject;

    GameObject player;

    public void AddItem()
    {
        FindObjectOfType<HDK_InventoryManager>().AddItem(itemName, itemInfo, itemIcon, itemType, AmmosQuantity, HealthValue, TargetObject, WeaponTarget);
    }

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (player)
        {
            endPos = Camera.main.transform.position + Camera.main.transform.forward * Distance;
        } 
    }

    public void Examine()
    {
        transform.position = Vector3.Lerp(startPos, endPos, 1);
    }

    public void RestorePos()
    {
        transform.position = Vector3.Lerp(endPos, startPos, 1);
        transform.rotation = startRot;
    }
}