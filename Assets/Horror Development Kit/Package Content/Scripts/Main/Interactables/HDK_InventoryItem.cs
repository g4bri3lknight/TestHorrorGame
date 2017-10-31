//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class HDK_InventoryItem : MonoBehaviour {

    [Header("Main Item Settings")]
    public string itemName;
    public string itemInfo;
    public ItemType itemType;
    public Sprite itemIcon;
    public bool ObjectKnown;

    [Header("Examination Settings")]
    public float Distance;
    public float MovementTime;
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
        //Move the item near the camera
        StartCoroutine(MoveToPosition(transform, endPos, MovementTime));
    }

    public void RestorePos()
    {
        //Restore the item world position
        StartCoroutine(MoveToPosition(transform, startPos, MovementTime));
        StartCoroutine(MoveToRotation(transform, startRot, MovementTime));
    }

    public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
    }

    public IEnumerator MoveToRotation(Transform transform, Quaternion rotation, float timeToMove)
    {
        var currentRot = transform.rotation;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.rotation = Quaternion.Lerp(currentRot, rotation, t);
            yield return null;
        }
    }
}
