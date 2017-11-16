using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;



public class ConsoleCommands:MonoBehaviour
{
    [Header("UI OBJ")]
    public HDK_InventoryManager inventoryManager;
    public GameObject InventoryScrollContentGUI;
    public GameObject slotMelee;
    public GameObject slotFiregun;
    public GameObject slotKey;


    [Header("Object References")]
    [Tooltip("Lista di oggetti padre in cu effettuare le ricerche. Se vuota verra' effettuata la ricerca solo al livello globale.")]
    public List<GameObject> listOfGameObjects;

    private string[] commandLine;

    private Command command;


    public string[] CommandLine
    {
        get
        {
            return this.commandLine;
        }
        set
        {
            commandLine = value;
        }
    }

    public Command Command
    {
        get
        {
            return this.command;
        }
        set
        {
            command = value;
        }
    }

    private GameObject Player;

    private int maxQta = 30;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }


    IEnumerator CreateObject()
    {
        if (commandLine.Length - 1 == command.NumberOfArguments)
        {
            string objectName = "";

            int quantity;

            objectName = commandLine[1].ToLower();

            string text = string.Format("<color=white>{0}</color>", objectName);

            if (!int.TryParse(commandLine[2], out quantity))
            {
                AddText(command.UsageHelp, "white");
                yield return new WaitForSeconds(0.2f);
            }  

            if (!CheckQuantityOfObject(quantity))
            {
                yield return new WaitForSeconds(0.2f);
            }

            List<GameObject> listaOggetti = GameObject.FindObjectsOfType(typeof(GameObject)).Cast<GameObject>().Where(g => g.name.ToLower() == objectName).ToList();

            if (listaOggetti.Count() > 0)
            {
                AddText("Info: " + text + " created!", "green");

                for (int i = 0; i < listaOggetti.Count(); i++)
                {
                    for (int j = 1; j <= quantity; j++)
                    {
                        Instantiate(listaOggetti[i], Player.transform.position + Player.transform.forward * 2, Player.transform.rotation);
                    }
                }
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                AddText("Error: object " + text + " not found!", "red");
                yield return new WaitForSeconds(0.2f);
            } 
        }
        else
        {
            AddText(command.UsageHelp, "white");
            yield return new WaitForSeconds(0.2f);
        }
    }


    public void Save()
    {
        SaveInventory();
        SavePlayerInfo();
        SaveObjectsToDestroy();
        SaveLoadManager.SaveScene();
    }

    public void Load()
    {
        //SaveLoadManager.LoadScene();
        LoadObjectsToDestroy();
        LoadInventory();
        LoadPlayerInfo();
       
    }



    public  void SaveInventory()
    {
        try
        {
            List<HDK_InventorySlot> listaOggettiInventario = InventoryScrollContentGUI.GetComponentsInChildren<HDK_InventorySlot>().ToList();

            if (!slotMelee.GetComponent<HDK_InventorySlot>().Empty)
            {
                listaOggettiInventario.Add(slotMelee.GetComponent<HDK_InventorySlot>());
            }

            if (!slotFiregun.GetComponent<HDK_InventorySlot>().Empty)
            {
                listaOggettiInventario.Add(slotFiregun.GetComponent<HDK_InventorySlot>());
            }

            if (!slotKey.GetComponent<HDK_InventorySlot>().Empty)
            {
                listaOggettiInventario.Add(slotKey.GetComponent<HDK_InventorySlot>());
            }

            SaveLoadManager.SaveInventory(listaOggettiInventario);
        }
        catch (Exception ex)
        {
            Debug.LogError("Errore in SaveInventory: " + ex);
        }
    }

    public  void LoadInventory()
    {
        List<ItemAttribute> listaOggetti = SaveLoadManager.LoadInventory();

        foreach (ItemAttribute itemAttr in listaOggetti)
        {
            Transform slotObj = InventoryScrollContentGUI.transform.Find(itemAttr.SlotName);

            if (slotObj)
            {                   
                loadSlot(slotObj.gameObject, itemAttr);
            }

            if (itemAttr.SlotName == slotMelee.name)
            {
                loadSlot(slotMelee, itemAttr);
                FindObjectOfType<HDK_WeaponsManager>().currentMelee = slotMelee.GetComponent<HDK_InventorySlot>().WeaponTarget;
            }

            if (itemAttr.SlotName == slotFiregun.name)
            {
                loadSlot(slotFiregun, itemAttr);
                FindObjectOfType<HDK_WeaponsManager>().currentGun = slotFiregun.GetComponent<HDK_InventorySlot>().WeaponTarget;
            }

            if (itemAttr.SlotName == slotKey.name)
            {
                loadSlot(slotKey, itemAttr);
            }
        }
    }


    public  void SavePlayerInfo()
    {
        HDK_PlayerHealth playerhealth = Player.GetComponent<HDK_PlayerHealth>();

        Transform playerTransform = Player.transform;

        HDK_Flashlight flashLight = Player.GetComponent<HDK_Flashlight>();

        PlayerInfoDB playerInfo = PlayerInfoDB.CreateInstance<PlayerInfoDB>();

        playerInfo.PositionInScene = playerTransform.position;
        playerInfo.RotationInScene = playerTransform.rotation;
        playerInfo.TotalHealth = playerhealth.Health;
        playerInfo.CurrentHealth = playerhealth.maxHealth;

        playerInfo.HasFlashLight = flashLight.hasFlashlight;
        playerInfo.FlashLightHealth = flashLight.health;
        playerInfo.FlashLightTotalHealth = flashLight.MaxHealth;

        playerInfo.CurrentScene = SceneManager.GetActiveScene().name;

        SaveLoadManager.SavePlayerInfo(playerInfo);
    }

    public  void LoadPlayerInfo()
    {
        PlayerInfoDB info = SaveLoadManager.LoadPlayerInfo();
        Player.GetComponent<HDK_PlayerHealth>().Health = info.CurrentHealth;
        Player.GetComponent<HDK_PlayerHealth>().maxHealth = info.TotalHealth;

        Player.transform.position = info.PositionInScene;

        Player.transform.rotation = info.RotationInScene;

        Player.GetComponent<HDK_Flashlight>().hasFlashlight = info.HasFlashLight;
        Player.GetComponent<HDK_Flashlight>().health = info.FlashLightHealth;
        Player.GetComponent<HDK_Flashlight>().MaxHealth = info.FlashLightTotalHealth;
    }


    public  void SaveObjectsToDestroy()
    {
        SaveLoadManager.SaveObjectsToDestroy();
    }

    public  void LoadObjectsToDestroy()
    {
        List<string> listOfObjectsToDestroy = SaveLoadManager.LoadObjectsToDestroy();

        foreach (string itemName in listOfObjectsToDestroy)
        {
            GameObject obj = FindGameObject(itemName);

            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }

    void loadSlot(GameObject slotObj, ItemAttribute itemAttr)
    {
        HDK_InventorySlot slot = slotObj.GetComponent<HDK_InventorySlot>();

        Image slotImage = slot.gameObject.GetComponent<Image>();
        slot.itemName = itemAttr.ItemName;
        slot.itemInfo = itemAttr.ItemInfo;
        slot.itemType = itemAttr.ItemType;

        Sprite img = Resources.Load<Sprite>(itemAttr.ItemImageName);

        slotImage.sprite = img;

        slot.AmmosQuantity = itemAttr.AmmosQuantity;

        string weaponTarget = itemAttr.WeaponTargetName;

        if (weaponTarget != "")
        {
            slot.WeaponTarget = FindGameObject(weaponTarget);
        }

        slot.HealthValue = itemAttr.HealthValue;

        string targetObj = itemAttr.TargetObjName;

        if (targetObj != "")
        {
            slot.TargetObject = FindGameObject(targetObj);
        } 

        slot.Empty = false;
    }


    public GameObject FindGameObject(string objectName)
    {
        GameObject found = GameObject.Find(objectName); 

        if (found)
        {
            return found;
        }
        else
        {
            foreach (GameObject go in listOfGameObjects)
            {
                found = go.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == objectName).gameObject;

                if (found)
                {
                    return found;
                }
            }
        }

        return null;
    }

    public void AddText(string textToAdd, string color)
    {
        GameObject outObj = GameObject.Find("Output");

        if (outObj)
        {
            Text output = GameObject.Find("Output").GetComponent<Text>();
            output.text += string.Format("\n<color={1}>{0}</color>", textToAdd, color);
        }    
    }

    bool CheckQuantityOfObject(int quantity)
    {
        if (quantity == 0)
        {
            AddText("Error: quantity must be greater than 0!", "red");
            return false;
        }
        else if (quantity > 30)
        {
            AddText(string.Format("Error: quantity must be less than {0}!", maxQta), "red");
            return false;
        }

        return true;
    }


    public HDK_InventorySlot getAmmosForFireGun()
    {
        try
        {
            string nomeArma = slotFiregun.GetComponent<HDK_InventorySlot>().WeaponTarget.name;
            List<HDK_InventorySlot> listaOggettiInventario = InventoryScrollContentGUI.GetComponentsInChildren<HDK_InventorySlot>().ToList();

            foreach (HDK_InventorySlot slot in listaOggettiInventario)
            {
                if (slot.WeaponTarget != null)
                {
                    if (slot.itemType == ItemType.Ammo && slot.WeaponTarget.name == nomeArma)
                    {
                        return slot;
                    }
                } 
            }
        }
        catch (NullReferenceException)
        {
            return null;
        }
            
        return null;
    }

    public int getTotalAmmosQuantityForFireGun()
    {
        int counTotalAmmo = 0;

        HDK_InventorySlot slot = getAmmosForFireGun();

        if (slot != null)
        {
            counTotalAmmo += slot.AmmosQuantity;
        }
            
        return counTotalAmmo;
    }

    public void setAmmosQuantityForFireGun(int newValue)
    {
        HDK_InventorySlot slot = getAmmosForFireGun();

        if (slot != null)
        {
            slot.AmmosQuantity = newValue;

            if (slot.AmmosQuantity == 0)
            {
                slot.ResetSlot();
                inventoryManager.freeSlots++;
            }
        }
    }

    public HDK_InventorySlot getSlotByItemName(string objectName)
    {
        List<HDK_InventorySlot> listaOggettiInventario = InventoryScrollContentGUI.GetComponentsInChildren<HDK_InventorySlot>().ToList();

        foreach (HDK_InventorySlot slot in listaOggettiInventario)
        {
            if (slot.itemName == objectName && (slot.itemType == ItemType.Ammo ||
                slot.itemType == ItemType.Eatable || slot.itemType == ItemType.FlashlightBatteries || slot.itemType == ItemType.Medikit))
            {
                return slot;
            }
        }

        return null;
    }
        
}
