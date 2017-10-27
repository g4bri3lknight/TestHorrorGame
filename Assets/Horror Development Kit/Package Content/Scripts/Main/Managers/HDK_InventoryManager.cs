//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;
using Lean.Localization;
using System.Linq;

public class HDK_InventoryManager : MonoBehaviour
{
    [Header("ConsoleCommands")]
    public ConsoleCommands consoleCommands;

    [Header("Loot")]
    public List<GameObject> inventorySlots;
    public int freeSlots;
    int totalSlots;
    public GameObject meleeSlot;
    public GameObject firegunSlot;
    public GameObject keySlot;

    [Header("Selected Loot Slot")]
    public GameObject selectedSlot;
    public ItemType selectedSlotType;
    string slotType;
    bool emptySlot;

    [Header("UI")]
    public GameObject Inventory;
    public GameObject[] ToDisableGUI;
    public Text itemName;
    public Text itemInfo;
    public Text itemType;
    public Text ammo_health_Value;
    public Text errorLine;
    public Text slotText;
    public GameObject InfoPanel;
    public GameObject buttonEquip;
    public GameObject buttonUse;

    public static bool inventoryOpen;
    GameObject Player;

    [Header("SFX")]
    public AudioClip openCloseSound;
    public AudioClip itemDestorySound;
    public AudioClip mouseHover;
    public AudioClip mouseClick;
    public float mouseVolume;
    AudioSource sourceAudio;




    public void EquipItem()
    {
        if (selectedSlot == meleeSlot || selectedSlot == firegunSlot || selectedSlot == keySlot)
        {
            if (!selectedSlot.GetComponent<HDK_InventorySlot>().Empty)
            {
                //StartCoroutine(ErrorText("ITEM ALREADY EQUIPPED"));
                DestroyItem();
            }
            else
            {
                // StartCoroutine(ErrorText("EMPTY SLOT"));
            }
        }
        else if (selectedSlot != null && selectedSlot != meleeSlot && selectedSlot != firegunSlot && selectedSlot != keySlot)
        {
            if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.NONE)
            {
                //StartCoroutine(ErrorText("EMPTY SLOT"));
            }
            if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Firegun)
            {
                HDK_InventorySlot selSlot = selectedSlot.GetComponent<HDK_InventorySlot>();
                Sprite itemIcon = selSlot.GetComponent<Image>().sprite;
                GameObject weaponTarget = selSlot.GetComponent<HDK_InventorySlot>().WeaponTarget;
                if (firegunSlot.GetComponent<HDK_InventorySlot>().Empty)
                {
                    firegunSlot.GetComponent<HDK_InventorySlot>().UpdateSlot(selSlot.itemName, selSlot.itemInfo, itemIcon, ItemType.Firegun, 0, 0, null, weaponTarget);
                    selSlot.ResetSlot();
                    freeSlots += 1;
                    FindObjectOfType<HDK_WeaponsManager>().currentGun = weaponTarget;

                    firegunSlot.GetComponent<Button>().Select();
                }
                else
                {
                    //StartCoroutine(ErrorText("YOU ARE ALREADY USING A FIREGUN"));
                }
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Melee)
            {
                if (meleeSlot.GetComponent<HDK_InventorySlot>().Empty)
                {
                    HDK_InventorySlot selSlot = selectedSlot.GetComponent<HDK_InventorySlot>();
                    Sprite itemIcon = selSlot.GetComponent<Image>().sprite;
                    GameObject weaponTarget = selSlot.GetComponent<HDK_InventorySlot>().WeaponTarget;
                    if (meleeSlot.GetComponent<HDK_InventorySlot>().Empty)
                    {
                        meleeSlot.GetComponent<HDK_InventorySlot>().UpdateSlot(selSlot.itemName, selSlot.itemInfo, itemIcon, ItemType.Melee, 0, 0, null, weaponTarget);
                        selSlot.ResetSlot();
                        freeSlots += 1;
                        FindObjectOfType<HDK_WeaponsManager>().currentMelee = weaponTarget;
                        meleeSlot.GetComponent<Button>().Select();
                    }
                }
                else
                {
                    //StartCoroutine(ErrorText("YOU ARE ALREADY USING A MELEE"));
                }
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Ammo)
            {
                //StartCoroutine(ErrorText("YOU DON'T NEED TO EQUIP THIS ITEM"));
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Flashlight)
            {
                //StartCoroutine(ErrorText("YOU DON'T NEED TO EQUIP THIS ITEM"));
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.FlashlightBatteries)
            {
                //StartCoroutine(ErrorText("YOU DON'T NEED TO EQUIP THIS ITEM"));
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.DigitalCamera)
            {
                //StartCoroutine(ErrorText("YOU DON'T NEED TO EQUIP THIS ITEM"));
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Medikit)
            {
                //StartCoroutine(ErrorText("YOU DON'T NEED TO EQUIP THIS ITEM"));
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Eatable)
            {
                //StartCoroutine(ErrorText("YOU DON'T NEED TO EQUIP THIS ITEM"));
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Key)
            {
                if (keySlot.GetComponent<HDK_InventorySlot>().Empty)
                {
                    HDK_InventorySlot selSlot = selectedSlot.GetComponent<HDK_InventorySlot>();
                    Sprite itemIcon = selSlot.GetComponent<Image>().sprite;
                    GameObject targetDoor = selSlot.GetComponent<HDK_InventorySlot>().TargetObject;
                    if (keySlot.GetComponent<HDK_InventorySlot>().Empty)
                    {
                        keySlot.GetComponent<HDK_InventorySlot>().UpdateSlot(selSlot.itemName, selSlot.itemInfo, itemIcon, ItemType.Key, 0, 0, targetDoor, null);
                        selSlot.ResetSlot();
                        freeSlots += 1;
                        keySlot.GetComponent<Button>().Select();
                    }
                }
                else
                {
                    StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("KeyAlreadyEquipped")));
                }
            }
        }
        else if (selectedSlot == null)
        {
            //StartCoroutine(ErrorText("YOU MUST SELECT AN ITEM"));
        }

        SlotSelection(false, selectedSlot);
    }

    public void UseItem()
    {
        if (selectedSlot == meleeSlot || selectedSlot == firegunSlot || selectedSlot == keySlot)
        {
            if (!selectedSlot.GetComponent<HDK_InventorySlot>().Empty)
            {
                if (selectedSlot == meleeSlot)
                {
                    StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("UseKeyToMelee")));
                }
                else if (selectedSlot == firegunSlot)
                {
                    StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("UseKeyToFireGun")));
                }
                else if (selectedSlot == keySlot)
                {
                    keySlot.GetComponent<HDK_InventorySlot>().TargetObject.SendMessage("UnlockDoor");
                    keySlot.GetComponent<HDK_InventorySlot>().ResetSlot();
                }
            }
            else
            {
                //StartCoroutine(ErrorText("EMPTY SLOT"));
            }
        }
        else if (selectedSlot != null && selectedSlot != meleeSlot && selectedSlot != firegunSlot && selectedSlot != keySlot)
        {
            HDK_InventorySlot selSlot = selectedSlot.GetComponent<HDK_InventorySlot>();

            if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.NONE)
            {
                //StartCoroutine(ErrorText("EMPTY SLOT"));
            }
            if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Firegun)
            {
                StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("NoFireGunEquipped")));
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Melee)
            {
                StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("NoMeleeEquipped")));
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Ammo)
            {
                if (firegunSlot.GetComponent<HDK_InventorySlot>().Empty)
                {
                    StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("NoFireGunEquipped")));
                }
                else if (firegunSlot.GetComponent<HDK_InventorySlot>().WeaponTarget == selectedSlot.GetComponent<HDK_InventorySlot>().WeaponTarget)
                {
                    // FindObjectOfType<HDK_WeaponsManager>().AddAmmos(selectedSlot.GetComponent<HDK_InventorySlot>().AmmosQuantity);
                    //selSlot.ResetSlot();
                    //freeSlots += 1;
                }
                else if (firegunSlot.GetComponent<HDK_InventorySlot>().WeaponTarget != selectedSlot.GetComponent<HDK_InventorySlot>().WeaponTarget)
                {
                    // StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("IncompatibleAmmo")));
                }
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Flashlight)
            {
                FindObjectOfType<HDK_Flashlight>().DrawFlashlight();
                CloseInventory();
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.FlashlightBatteries)
            {
                if (FindObjectOfType<HDK_WeaponsManager>().usingFlashlight)
                {
                    if (FindObjectOfType<HDK_Flashlight>().health <= 80)
                    {
                        FindObjectOfType<HDK_Flashlight>().Recharge();
                        selSlot.ResetSlot();
                        freeSlots += 1;
                    }
                    else
                    {
                        StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("HealthFlashLightFull")));
                    }
                }
                else
                {
                    StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("NoFlashLightEquipped")));
                }
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.DigitalCamera)
            {
                CloseInventory();
                if (!FindObjectOfType<HDK_DigitalCamera>().UsingCamera)
                {
                    FindObjectOfType<HDK_DigitalCamera>().CameraUse(true);
                }
                else
                {
                    FindObjectOfType<HDK_DigitalCamera>().CameraUse(false);
                }
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Medikit)
            {
                if (selectedSlot.GetComponent<HDK_InventorySlot>().HealthValue > 0)
                {
                    if (Player.GetComponent<HDK_PlayerHealth>().Health < 100f)
                    {
                        Player.GetComponent<HDK_PlayerHealth>().Health += selectedSlot.GetComponent<HDK_InventorySlot>().HealthValue;
                        selSlot.ResetSlot();
                        freeSlots += 1;
                    }
                    else
                    {
                        StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("HealthFull"))); 
                    }
                }
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Eatable)
            {
                if (selectedSlot.GetComponent<HDK_InventorySlot>().HealthValue > 0)
                {
                    if (Player.GetComponent<HDK_PlayerHealth>().Health < 100f)
                    {
                        Player.GetComponent<HDK_PlayerHealth>().Health += selectedSlot.GetComponent<HDK_InventorySlot>().HealthValue;
                        selSlot.ResetSlot();
                        freeSlots += 1;
                    }
                    else
                    {
                        StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("HealthFull")));
                    }
                }
                else
                {
                    Player.GetComponent<HDK_PlayerHealth>().FallingDamage(selectedSlot.GetComponent<HDK_InventorySlot>().HealthValue);
                }
            }
            else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Key)
            {
                StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("ItemOnlyEquippable")));
            }
        }
        else if (selectedSlot == null)
        {
            //StartCoroutine(ErrorText("YOU MUST SELECT AN ITEM"));
        }
    }

    IEnumerator ErrorText(string error)
    {
        errorLine.text = error;
        errorLine.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        yield return new WaitForSeconds(2);
        errorLine.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void DestroyItem()
    {
        if (selectedSlot == null)
        {
            //StartCoroutine(ErrorText("YOU MUST SELECT AN ITEM"));
        }
        else if (selectedSlot.GetComponent<HDK_InventorySlot>().Empty)
        {
            //StartCoroutine(ErrorText("EMPTY SLOT"));
        }
        else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Flashlight)
        {
            if (FindObjectOfType<HDK_Flashlight>().usingFlashlight)
            {
                FindObjectOfType<HDK_Flashlight>().PutdownFlashlight(0);
            }
            selectedSlot.GetComponent<HDK_InventorySlot>().ResetSlot();
            freeSlots += 1;
            FindObjectOfType<HDK_Flashlight>().hasFlashlight = false;
            sourceAudio.clip = itemDestorySound;
            sourceAudio.Play();
        }
        else if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.DigitalCamera)
        {
            if (FindObjectOfType<HDK_DigitalCamera>().UsingCamera)
            {
                FindObjectOfType<HDK_DigitalCamera>().CameraUse(false);
            }
            selectedSlot.GetComponent<HDK_InventorySlot>().ResetSlot();
            freeSlots += 1;
            FindObjectOfType<HDK_DigitalCamera>().HasCamera = false;
            sourceAudio.clip = itemDestorySound;
            sourceAudio.Play();
        }
        //-------------------DA MODIFICARE!!!!----------------------
        else if (selectedSlot != meleeSlot && selectedSlot != firegunSlot && selectedSlot != keySlot)
        {
            if (selectedSlot != null && !selectedSlot.GetComponent<HDK_InventorySlot>().Empty)
            {
                selectedSlot.GetComponent<HDK_InventorySlot>().ResetSlot();
                freeSlots += 1;
                sourceAudio.clip = itemDestorySound;
                sourceAudio.Play();
            }
        }
        else if (selectedSlot != null && !selectedSlot.GetComponent<HDK_InventorySlot>().Empty)
        {
            if (freeSlots > 0)
            {
                if (selectedSlot == meleeSlot)
                {
                    string name = meleeSlot.GetComponent<HDK_InventorySlot>().itemName;
                    string info = meleeSlot.GetComponent<HDK_InventorySlot>().itemInfo;
                    Sprite icon = meleeSlot.GetComponent<Image>().sprite;
                    ItemType type = meleeSlot.GetComponent<HDK_InventorySlot>().itemType;
                    GameObject weapon = meleeSlot.GetComponent<HDK_InventorySlot>().WeaponTarget;
                    AddItem(name, info, icon, type, 0, 0, null, weapon);
                    meleeSlot.GetComponent<HDK_InventorySlot>().ResetSlot();
                    FindObjectOfType<HDK_WeaponsManager>().melee_Putdown(false, true);
                }
                else if (selectedSlot == firegunSlot)
                {
                    string name = firegunSlot.GetComponent<HDK_InventorySlot>().itemName;
                    string info = firegunSlot.GetComponent<HDK_InventorySlot>().itemInfo;
                    Sprite icon = firegunSlot.GetComponent<Image>().sprite;
                    ItemType type = firegunSlot.GetComponent<HDK_InventorySlot>().itemType;
                    GameObject weapon = firegunSlot.GetComponent<HDK_InventorySlot>().WeaponTarget;
                    AddItem(name, info, icon, type, 0, 0, null, weapon);
                    firegunSlot.GetComponent<HDK_InventorySlot>().ResetSlot();
                    FindObjectOfType<HDK_WeaponsManager>().gun_Putdown(false, true);
                }
                else if (selectedSlot == keySlot)
                {
                    string name = keySlot.GetComponent<HDK_InventorySlot>().itemName;
                    string info = keySlot.GetComponent<HDK_InventorySlot>().itemInfo;
                    Sprite icon = keySlot.GetComponent<Image>().sprite;
                    ItemType type = keySlot.GetComponent<HDK_InventorySlot>().itemType;
                    GameObject targetDoor = keySlot.GetComponent<HDK_InventorySlot>().TargetObject;
                    AddItem(name, info, icon, type, 0, 0, null, targetDoor);
                    keySlot.GetComponent<HDK_InventorySlot>().ResetSlot();
                }
            }
            else
            {
                StartCoroutine(ErrorText(LeanLocalization.GetTranslationText("InventoryFull")));
            }
        }
    }

    public void AddItem(string name, string info, Sprite icon, ItemType type, int ammo, float health, GameObject door, GameObject weapon)
    {
        HDK_InventorySlot slot = consoleCommands.getSlotByItemName(name);

        if (slot != null)
        {
            //se l'oggetto esiste incremento solo la quantita'
            slot.AmmosQuantity += ammo;
        }
        else
        {//se l'oggetto non esiste lo creo
            if (freeSlots > 0)
            {
                for (int i = 0; i <= inventorySlots.Count; i++)
                {
                    if (inventorySlots[i] == inventorySlots[i].GetComponent<HDK_InventorySlot>().Empty)
                    {
                        inventorySlots[i].GetComponent<HDK_InventorySlot>().UpdateSlot(name, info, icon, type, ammo, health, door, weapon);
                        freeSlots -= 1;
                        break;
                    }
                }
            }
            else
            {
                //No space in the inventory
            }
        }
    }

    public void DeselectItem()
    {
        SlotSelection(true, null);
    }

    public void SlotSelection(bool empty, GameObject slot)
    {
        selectedSlot = slot;
        if (empty)
        {
            emptySlot = true;
        }
        else
        {
            emptySlot = false;
        }
    }

    public void PlayHover()
    {
        GetComponent<AudioSource>().PlayOneShot(mouseHover, mouseVolume);
    }

    public void PlayClick()
    {
        GetComponent<AudioSource>().PlayOneShot(mouseClick, mouseVolume);
    }

    void Start()
    {
        Player = GameObject.Find("Player");
        totalSlots = inventorySlots.Count;
        freeSlots = totalSlots;
        sourceAudio = GetComponent<AudioSource>();
    }

    public void CloseInventory()
    {
        Player.GetComponentInChildren<BlurOptimized>().enabled = false;
        Player.GetComponentInChildren<HDK_RaycastManager>().enabled = true;
        Player.GetComponent<HDK_MouseZoom>().canZoom = true;
        Player.GetComponentInChildren<HeadBobController>().enabled = true;
        Player.GetComponentInChildren<SwayWeapon>().enabled = true;
        Player.GetComponent<HDK_Stamina>().Busy(false);
        Inventory.SetActive(false);
        Player.GetComponent<FirstPersonController>().enabled = true;
        inventoryOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        sourceAudio.clip = openCloseSound;
        sourceAudio.Play();
        foreach (GameObject obj in ToDisableGUI)
        {
            obj.SetActive(true);
        }
    }

    public void OpenInventory()
    {
        Player.GetComponentInChildren<BlurOptimized>().enabled = true;
        Player.GetComponentInChildren<HDK_RaycastManager>().enabled = false;
        Player.GetComponent<HDK_MouseZoom>().ZoomOut();
        Player.GetComponentInChildren<SwayWeapon>().enabled = false;
        Player.GetComponentInChildren<HeadBobController>().enabled = false;
        Player.GetComponent<HDK_Stamina>().Busy(true);
        Player.GetComponent<FirstPersonController>().enabled = false;
        Inventory.SetActive(true);
        inventoryOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        sourceAudio.clip = openCloseSound;
        sourceAudio.Play();    
        foreach (GameObject obj in ToDisableGUI)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        if (selectedSlot != null)
        {
            if (selectedSlot.GetComponent<HDK_InventorySlot>().Empty)
            {
                emptySlot = true;
                InfoPanel.GetComponent<Animator>().SetBool("visible", false);
                //InfoPanel.SetActive(false);
            }
            else //++++++MODIFICA++++++
            {
                if (selectedSlot.GetComponent<Button>().IsActive())
                {
                    InfoPanel.SetActive(true);
                    InfoPanel.GetComponent<Animator>().SetBool("visible", true);

                    if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Firegun ||
                        selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Melee ||
                        selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Key)
                    {
                        buttonEquip.SetActive(true);
                        buttonUse.SetActive(false);
                    }

                    if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Eatable ||
                        selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.FlashlightBatteries ||
                        selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Medikit ||
                        selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.DigitalCamera ||
                        selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Flashlight)
                    {
                        buttonEquip.SetActive(false);
                        buttonUse.SetActive(true);
                    }

                    if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Ammo)
                    {
                        buttonEquip.SetActive(false);
                        buttonUse.SetActive(false);
                    }
                }
            }
        }


        bool examining = HDK_RaycastManager.ExaminingObject;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool paused = HDK_PauseManager.GamePaused;

        if (!paused && !examining && !reading && !security && !inventoryOpen)
        {
            //if (Input.GetKeyDown(KeyCode.Tab))
            if (Input.GetButtonDown("Inventary"))
            {
                OpenInventory();
            }
        }
        else if (examining)
        {
            if (Input.GetButtonDown("Inventary"))
            {
                HDK_WeaponsManager wepMan = FindObjectOfType<HDK_WeaponsManager>();
                if (wepMan.usingGun)
                {
                    wepMan.currentGun.SetActive(true);
                }
                else if (wepMan.usingMelee)
                {
                    wepMan.currentMelee.SetActive(true);
                }
                else if (wepMan.usingFlashlight)
                {
                    Player.GetComponent<HDK_Flashlight>().ArmsAnims.SetActive(true);
                }
                Player.GetComponentInChildren<HDK_RaycastManager>().PutDownObject();
                OpenInventory();
            }
        }
        else if (reading)
        {
            if (Input.GetButtonDown("Inventary"))
            {
                Player.GetComponentInChildren<HDK_RaycastManager>().ClosePaper();
                OpenInventory();
            }
        }
        else if (security)
        {
            if (Input.GetButtonDown("Inventary"))
            {
                Player.GetComponentInChildren<HDK_RaycastManager>().CloseCam();
                OpenInventory();
            }
        }
        else if (paused)
        {
            if (Input.GetButtonDown("Inventary"))
            {
                FindObjectOfType<HDK_PauseManager>().UnPause();
                OpenInventory();
            }
        }
        else if (inventoryOpen)
        {
            if (Input.GetButtonDown("Inventary"))
            {
                CloseInventory();
            }
        }

        switch (selectedSlotType)
        {
            case ItemType.NONE:
                slotType = LeanLocalization.GetTranslationText("SlotTypeNone");
                break;

            case ItemType.Firegun:
                slotType = LeanLocalization.GetTranslationText("SlotTypeFireGun");
                break;

            case ItemType.Melee:
                slotType = LeanLocalization.GetTranslationText("SlotTypeMelee");
                break;

            case ItemType.Ammo:
                slotType = LeanLocalization.GetTranslationText("SloTypeAmmo");
                break;

            case ItemType.Flashlight:
                slotType = LeanLocalization.GetTranslationText("SlotTypeFlashLight");
                break;

            case ItemType.FlashlightBatteries:
                slotType = LeanLocalization.GetTranslationText("SlotTypeFlashLighttBattery");
                break;

            case ItemType.DigitalCamera:
                slotType = LeanLocalization.GetTranslationText("SlotTypeCamera");
                break;

            case ItemType.Medikit:
                slotType = LeanLocalization.GetTranslationText("SlotTypeMediKit");
                break;

            case ItemType.Eatable:
                slotType = LeanLocalization.GetTranslationText("SlotTypeEatable");
                break;

            case ItemType.Key:
                slotType = LeanLocalization.GetTranslationText("SlotTypeKey");
                break;
        }

        CalculateTotalAndFreeSlot();

        slotText.text = freeSlots.ToString() + " / " + totalSlots.ToString();

        if (selectedSlot == null)
        {
            itemName.text = LeanLocalization.GetTranslationText("NoItemSelected");
            itemInfo.text = LeanLocalization.GetTranslationText("NoItemSelected");
            itemType.text = LeanLocalization.GetTranslationText("NoItemSelected");
            ammo_health_Value.text = LeanLocalization.GetTranslationText("NoItemSelected");
        }
        else if (emptySlot)
        {
            itemName.text = LeanLocalization.GetTranslationText("NoItemSelected");
            itemInfo.text = LeanLocalization.GetTranslationText("NoItemSelected");
            itemType.text = LeanLocalization.GetTranslationText("NoItemSelected");
            ammo_health_Value.text = LeanLocalization.GetTranslationText("NoItemSelected");
        }
        else if (!emptySlot && selectedSlot != null)
        {
            itemName.text = selectedSlot.GetComponent<HDK_InventorySlot>().itemName;
            itemInfo.text = selectedSlot.GetComponent<HDK_InventorySlot>().itemInfo;
            itemType.text = slotType;

            if (selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Ammo
                || selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Firegun
                || selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Medikit
                || selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.Eatable
                || selectedSlot.GetComponent<HDK_InventorySlot>().itemType == ItemType.FlashlightBatteries)
            {
                ammo_health_Value.text = selectedSlot.GetComponent<HDK_InventorySlot>().AmmosQuantity.ToString();
            }
            else
            {
                ammo_health_Value.text = "N/D";
            }
        }
    }


    void CalculateTotalAndFreeSlot()
    {
        totalSlots = this.inventorySlots.Count;

        freeSlots = this.inventorySlots.Count(x => x.GetComponent<HDK_InventorySlot>().Empty);
    }
}