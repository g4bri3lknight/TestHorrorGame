//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    NONE,
    Firegun,
    Melee,
    Ammo,
    Flashlight,
    FlashlightBatteries,
    DigitalCamera,
    Medikit,
    Eatable,
    Key,
    OnlyExamination}
;

public class HDK_InventorySlot : MonoBehaviour
{

    [Header("Main Item Settings")]
    public string itemName;
    public string itemInfo;
    public ItemType itemType;
    public Sprite defaultIcon;

    [Header("Ammos Settings")]
    public int AmmosQuantity;

    [Header("Weapon Settings")]
    public GameObject WeaponTarget;

    [Header("Eatable Settings")]
    public float HealthValue;

    [Header("Key Settings")]
    public GameObject TargetObject;
    
    public bool Empty;
    HDK_InventoryManager inventoryManager;

    private Text textQuantity;

    private int ammoInitialAmount;

    void Update()
    {
        if (textQuantity != null)
        {
            if (itemType != ItemType.DigitalCamera || itemType != ItemType.Flashlight || itemType != ItemType.Key || itemType != ItemType.Melee || itemType != ItemType.Melee)
            {
                setTextQuantity(AmmosQuantity);
            }    
        }
    }

    void setTextQuantity(int qta)
    {
        if (!Empty)
        {
            if (itemType == ItemType.Firegun)
            {
                textQuantity.enabled = true;

                if (qta > 6)
                {
                    textQuantity.color = Color.white;
                }
                else
                {
                    textQuantity.color = Color.red; 
                }

                textQuantity.text = qta.ToString();
            }
            else
            {
                if (qta > 1)
                {
                    textQuantity.text = qta.ToString();
                    textQuantity.enabled = true;
                    textQuantity.color = Color.white;
                }
            }
        }
        else
        {
            textQuantity.enabled = false;
        }
    }

    private void Start()
    {
        inventoryManager = GetComponentInParent<HDK_InventoryManager>();

        textQuantity = this.gameObject.GetComponentInChildren<Text>();
    }

    public void UpdateSlot(string name, string info, Sprite icon, ItemType type, int ammos, float health, GameObject door, GameObject weapon)
    {
        GetComponent<Image>().sprite = icon;
        itemName = name;
        itemInfo = info;
        itemType = type;
        AmmosQuantity = ammos;
        HealthValue = health;
        TargetObject = door;
        WeaponTarget = weapon;
        if (itemType == ItemType.NONE)
        {
            Empty = true;
            GetComponent<Image>().sprite = defaultIcon;
        }
        else
        {
            Empty = false;
        }
    }

    public void CheckSlot()
    {
        inventoryManager.SlotSelection(Empty, this.gameObject);
        inventoryManager.selectedSlotType = itemType;
    }

    public void Deselected()
    {
        inventoryManager.SlotSelection(Empty, null);
    }

    public void ResetSlot()
    {
        UpdateSlot(null, null, null, ItemType.NONE, 0, 0, null, null);
        Empty = true;
        GetComponent<Image>().sprite = defaultIcon;
    }

    public void SetAmmosQuantity(int quantity)
    {
        AmmosQuantity = quantity;
    }
}