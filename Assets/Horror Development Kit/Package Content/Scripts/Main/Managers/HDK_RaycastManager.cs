//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using Lean.Localization;

[System.Serializable]
public class DoorGrabClass
{
    public float DoorPickupRange = 2f;
    public float DoorThrow = 10f;
    public float DoorDistance = 2f;
    public float DoorMaxGrab = 3f;
    public AudioClip Locked;
}

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class HDK_RaycastManager : MonoBehaviour
    {

        [Header("Crosshair")]
        public GameObject normal_Crosshair;
        public GameObject interact_Crosshair;

        [Header("Raycast")]
        public float distance = 2.0f;
        public LayerMask layerMaskInteract;
        public Color RayGizmoColor;
        RaycastHit hit;

        [Header("Tags")]
        string KeyTag = "Key";
        string FlashlightTag = "Flashlight";
        string FlashlightBatteryTag = "FlashlightBattery";
        string DoorTag = "Door";
        string PaperTag = "Paper";
        string TelecameraTag = "Telecamera";
        string LampTag = "Lamp";
        string PlayAudioTag = "PlayAudio";
        string SecurityCamTag = "SecurityCamera";
        string FoodTag = "Food";
        string WeaponTag = "Weapon";
        string AmmoTag = "Ammo";
        string ExamineTag = "Examine";

        [Header("SFX")]
        public AudioClip[] ItemPickup;
        public AudioClip[] PaperInteract;
        public AudioClip[] ExaminedReveal;
        public AudioClip InventoryFull;
        public float pickupVolume;
        public float revealVolume;

        [Header("Tags Bools")]
        bool OnTagKey;
        bool OnTagFlashlight;
        bool OnTagFlashlightBattery;
        bool OnTagDoor;
        bool OnTagPaper;
        bool OnTagTelecamera;
        bool OnTagLamp;
        bool OnTagPlayAudio;
        bool OnTagSecurityCam;
        bool OnTagFood;
        bool OnTagWeapon;
        bool OnTagAmmo;
        bool OnTagExamination;

        [Header("Other")]
        GameObject Player;
        //Player GameObject
        bool hasFlashlight;
        //Do we have the flashlight?
        GameObject targetPaperNote;
        //The target paper note
        public GameObject raycasted_obj;
        //The raycasted item
        GameObject RaycastedLamp;
        //The raycasted functional lamp
        public static bool ExaminingObject;
        //Are we examining an item?
        public static bool ReadingPaper;
        //Are we reading a paper?
        public static bool UsingSecurityCam;
        //Are we using a security camera?
        public static bool raycastingObj;
        //Are we raycasting some interactable object?
        GameObject ExamineObjectInfoGUI;
        //The Examine Object Info GUI
        GameObject InteractInfoGui;
        //The Examine Object Info GUI
        bool ShowExaminingInfoGui;
        //Do we need to show the Examine Object Info GUI?
        public float RevealWait;
        //The time we need to wait for the item to be revealed after the examination
        GameObject ItemNameText;
        //The text that shows us the name of the item
        bool FadeInteractInfoGUI;
        //Do we need to fade the Interact Info GUI?
        GameObject examineEyeIcon;
        bool hasPeak;
        bool hasHBob;
        HDK_WeaponsManager weaponManager;
        bool examining;
        Light examinationLight;
        Camera playerCam;
        string furnitureRaycasted;

        [Header("Draggable Door Settings")]
        public DoorGrabClass DoorGrab = new DoorGrabClass();
        private float maxDistanceGrab = 4f;
        private Ray playerAim;
        private GameObject objectHeld;
        private bool isObjectHeld;
        private bool tryPickupObject;

        void Start()
        {
            Player = GameObject.Find("Player");
            ExaminingObject = false;
            ExamineObjectInfoGUI = GameObject.Find("examineControls");
            InteractInfoGui = GameObject.Find("interactControl");
            ItemNameText = GameObject.Find("itemName");
            examineEyeIcon = GameObject.Find("icon_examining");
            weaponManager = FindObjectOfType<HDK_WeaponsManager>().GetComponent<HDK_WeaponsManager>();
            examinationLight = GameObject.Find("ExaminationLight").GetComponent<Light>();
            playerCam = GameObject.Find("Camera").GetComponent<Camera>();
            isObjectHeld = false;
            tryPickupObject = false;
            objectHeld = null;
        }

        IEnumerator RevealExamined()
        {
            yield return new WaitForSeconds(RevealWait);
            if (ExaminingObject)
            {
                ItemNameText.GetComponent<Text>().text = raycasted_obj.GetComponent<HDK_InventoryItem>().itemName;
                raycasted_obj.GetComponent<HDK_InventoryItem>().ObjectKnown = true;
                GetComponent<AudioSource>().clip = ExaminedReveal[Random.Range(0, ExaminedReveal.Length)];
                GetComponent<AudioSource>().volume = revealVolume;
                GetComponent<AudioSource>().Play();
                FadeInteractInfoGUI = true;
            }
        }

        void FixedUpdate()
        {
            if (furnitureRaycasted == "DraggableDoor")
            {
                if (Input.GetButton("Interact"))
                {
                    if (!isObjectHeld)
                    {
                        tryPickObject();
                        tryPickupObject = true;
                    }
                    else
                    {
                        holdObject();
                    }
                }
                else if (isObjectHeld)
                {
                    DropObject();
                }
            }
        }

        void Update()
        {

            examining = ExaminingObject;

            if (FadeInteractInfoGUI)
            {
                ItemNameText.GetComponent<HDK_UIFade>().FadeIn();
            }
            else
            {
                if (!ExaminingObject)
                {
                    ItemNameText.GetComponent<HDK_UIFade>().FadeOut();
                }
            }

            if (ShowExaminingInfoGui)
            {
                ExamineObjectInfoGUI.GetComponent<HDK_UIFade>().FadeIn();
            }
            else
            {
                ExamineObjectInfoGUI.GetComponent<HDK_UIFade>().FadeOut();
            }
            
            bool paused = HDK_PauseManager.GamePaused;

            Vector3 position = transform.parent.position;
            Vector3 direction = transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(transform.position, direction * distance, RayGizmoColor);

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.gameObject.GetComponent<HDK_InventoryItem>() || hit.transform.gameObject.GetComponent<HDK_InteractObject>())
                {
                    raycasted_obj = hit.transform.gameObject;
                    if (raycasted_obj.GetComponent<HDK_WithEmission>())
                    {
                        if (!examining)
                        {
                            raycasted_obj.GetComponent<HDK_WithEmission>().rayed = true;
                        }
                        else
                        {
                            raycasted_obj.GetComponent<HDK_WithEmission>().rayed = false;
                        }
                    }
                    else if (!raycasted_obj.GetComponent<HDK_WithEmission>() && raycasted_obj.GetComponentInChildren<Light>() && !raycasted_obj.GetComponent<HDK_WithNoEmission>())
                    {
                        if (!examining)
                        {
                            raycasted_obj.GetComponentInChildren<Light>().enabled = true;
                        }
                        else
                        {
                            raycasted_obj.GetComponentInChildren<Light>().enabled = false;
                        }
                    }
                    normal_Crosshair.SetActive(false);
                    interact_Crosshair.SetActive(true);
                    if (!ExaminingObject)
                    {
                        InteractInfoGui.GetComponent<HDK_UIFade>().FadeIn();
                    }
                    else
                    {
                        InteractInfoGui.GetComponent<HDK_UIFade>().FadeOut();
                    }
                    raycastingObj = true;
                    FadeInteractInfoGUI = true;

                    if (raycasted_obj.GetComponent<HDK_InventoryItem>())
                    {
                        if (raycasted_obj.GetComponent<HDK_InventoryItem>().ObjectKnown)
                        {
                            ItemNameText.GetComponent<Text>().text = raycasted_obj.GetComponent<HDK_InventoryItem>().itemName;
                        }
                        else
                        {
                            ItemNameText.GetComponent<Text>().text = null;
                        }
                    }
                    else
                    {
                        if (raycasted_obj.GetComponent<HDK_InteractObject>())
                        {
                            if (raycasted_obj.GetComponent<HDK_InteractObject>().ItemName != null)
                            {
                                ItemNameText.GetComponent<Text>().text = raycasted_obj.GetComponent<HDK_InteractObject>().ItemName;
                            }
                            else
                            {
                                ItemNameText.GetComponent<Text>().text = null;
                            }
                        }
                    }
                }
            }
            else
            {
                if (raycasted_obj != null)
                {
                    if (raycasted_obj.GetComponent<HDK_WithEmission>())
                    {
                        raycasted_obj.GetComponent<HDK_WithEmission>().rayed = false;
                    }
                    else if (!raycasted_obj.GetComponent<HDK_WithEmission>() && raycasted_obj.GetComponentInChildren<Light>() && !raycasted_obj.GetComponent<HDK_WithNoEmission>())
                    {
                        raycasted_obj.GetComponentInChildren<Light>().enabled = false;
                    }
                }
                normal_Crosshair.SetActive(true);
                interact_Crosshair.SetActive(false);
                InteractInfoGui.GetComponent<HDK_UIFade>().FadeOut();
                FadeInteractInfoGUI = false;
                raycastingObj = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(KeyTag) && !paused)
                {
                    OnTagKey = true;
                }
                else
                {
                    OnTagKey = false;
                }
            }
            else
            {
                OnTagKey = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(FlashlightTag) && !paused)
                {
                    OnTagFlashlight = true;
                }
                else
                {
                    OnTagFlashlight = false;
                }
            }
            else
            {
                OnTagFlashlight = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(FlashlightBatteryTag) && !paused)
                {
                    OnTagFlashlightBattery = true;
                }
                else
                {
                    OnTagFlashlightBattery = false;
                }
            }
            else
            {
                OnTagFlashlightBattery = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(DoorTag) && !paused)
                {
                    OnTagDoor = true;
                }
                else
                {
                    OnTagDoor = false;
                }
            }
            else
            {
                OnTagDoor = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(PaperTag) && !paused)
                {
                    targetPaperNote = hit.transform.GetComponent<HDK_Note>().UI_Note;
                    OnTagPaper = true;
                }
                else
                {
                    OnTagPaper = false;
                }
            }
            else
            {
                OnTagPaper = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(TelecameraTag) && !paused)
                {
                    OnTagTelecamera = true;
                }
                else
                {
                    OnTagTelecamera = false;
                }
            }
            else
            {
                OnTagTelecamera = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(LampTag) && hit.transform.GetComponent<HDK_SwitchableLamp>() && !paused)
                {
                    OnTagLamp = true;
                    RaycastedLamp = hit.transform.gameObject;
                }
                else
                {
                    OnTagLamp = false;
                }
            }
            else
            {
                OnTagLamp = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(PlayAudioTag) && !paused)
                {
                    OnTagPlayAudio = true;
                }
                else
                {
                    OnTagPlayAudio = false;
                }
            }
            else
            {
                OnTagPlayAudio = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(SecurityCamTag) && !paused)
                {
                    OnTagSecurityCam = true;
                }
                else
                {
                    OnTagSecurityCam = false;
                }
            }
            else
            {
                OnTagSecurityCam = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(FoodTag) && !paused)
                {
                    OnTagFood = true;
                }
                else
                {
                    OnTagFood = false;
                }
            }
            else
            {
                OnTagFood = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(WeaponTag) && !paused)
                {
                    OnTagWeapon = true;
                }
                else
                {
                    OnTagWeapon = false;
                }
            }
            else
            {
                OnTagWeapon = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(AmmoTag) && !paused)
                {
                    OnTagAmmo = true;
                }
                else
                {
                    OnTagAmmo = false;
                }
            }
            else
            {
                OnTagAmmo = false;
            }

            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
                if (hit.transform.CompareTag(ExamineTag) && hit.transform.GetComponent<HDK_InventoryItem>().itemType == ItemType.OnlyExamination && !paused)
                {
                    OnTagExamination = true;
                }
                else
                {
                    OnTagExamination = false;
                }
            }
            else
            {
                OnTagExamination = false;
            }

            if (OnTagAmmo || OnTagFlashlight || OnTagFlashlightBattery || OnTagFood || OnTagKey || OnTagTelecamera || OnTagWeapon || OnTagExamination)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (!ExaminingObject)
                    {
                        raycasted_obj.SendMessage("Examine");
                        if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                        {
                            Player.GetComponent<HDK_DigitalCamera>().CameraUI.GetComponent<CanvasGroup>().alpha = 0;
                            if (Player.GetComponent<HDK_DigitalCamera>().broken)
                            {
                                Player.GetComponent<HDK_DigitalCamera>().brokenGUI.GetComponent<CanvasGroup>().alpha = 0;
                                Player.GetComponent<HDK_DigitalCamera>().camera_effect.enabled = false;
                            }
                        }
                        if (!raycasted_obj.GetComponent<HDK_InventoryItem>().ObjectKnown)
                        {
                            StartCoroutine(RevealExamined());
                        }
                        if (GetComponentInParent<HeadBobController>() != null)
                        {
                            hasHBob = true;
                            GetComponentInParent<HeadBobController>().enabled = false;
                        }
                        else
                        {
                            hasHBob = false;
                        }
                        if (GetComponentInParent<HDK_Leaning>() != null)
                        {
                            hasPeak = true;
                            GetComponentInParent<HDK_Leaning>().enabled = false;
                        }
                        else
                        {
                            hasPeak = false;
                        }
                        if (Player.GetComponent<HDK_Stamina>() != null)
                        {
                            Player.GetComponent<HDK_Stamina>().Busy(true);
                        }
                        examineEyeIcon.GetComponent<HDK_UIFade>().FadeIn();
                        ShowExaminingInfoGui = true;
                        ExaminingObject = true;
                        examinationLight.enabled = true;
                        if (weaponManager.usingGun)
                        {
                            weaponManager.gun_Putdown(false, false);
                        }
                        else if (weaponManager.usingMelee)
                        {
                            weaponManager.melee_Putdown(false, false);
                        }
                        else if (weaponManager.usingFlashlight)
                        {
                            Player.GetComponent<HDK_Flashlight>().PutdownFlashlight(0);
                        }
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.None;
                        Player.GetComponent<FirstPersonController>().enabled = false;
                        Player.GetComponentInChildren<HDK_ExamineRotation>().enabled = true;
                        Player.GetComponentInChildren<HDK_ExamineRotation>().target = raycasted_obj.transform;
                        Player.GetComponent<HDK_DigitalCamera>().canZoom = false;
                    }
                    else
                    {
                        raycasted_obj.SendMessage("RestorePos");
                        if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                        {
                            Player.GetComponent<HDK_DigitalCamera>().CameraUI.GetComponent<CanvasGroup>().alpha = 1;
                            if (Player.GetComponent<HDK_DigitalCamera>().broken)
                            {
                                Player.GetComponent<HDK_DigitalCamera>().camera_effect.enabled = true;
                                Player.GetComponent<HDK_DigitalCamera>().brokenGUI.GetComponent<CanvasGroup>().alpha = 1;
                            }
                        }
                        if (hasHBob)
                        {
                            GetComponentInParent<HeadBobController>().enabled = true;
                        }
                        if (hasPeak)
                        {
                            GetComponentInParent<HDK_Leaning>().enabled = true;
                        }
                        if (Player.GetComponent<HDK_Stamina>() != null)
                        {
                            Player.GetComponent<HDK_Stamina>().Busy(false);
                        }
                        examineEyeIcon.GetComponent<HDK_UIFade>().FadeOut();
                        ExaminingObject = false;
                        examinationLight.enabled = false;
                        ShowExaminingInfoGui = false;
                        Player.GetComponent<FirstPersonController>().enabled = true;
                        Player.GetComponentInChildren<HDK_ExamineRotation>().enabled = false;
                        Player.GetComponentInChildren<HDK_ExamineRotation>().target = null;
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera && !Player.GetComponent<HDK_DigitalCamera>().broken)
                        {
                            Player.GetComponentInChildren<HDK_DigitalCamera>().canZoom = true;
                        }
                        if (!Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                        {
                            Player.GetComponent<HDK_MouseZoom>().canZoom = true;
                        }
                    }
                }
                if (ExaminingObject)
                {
                    if (Input.GetButtonDown("Submit"))
                    {
                        if (!OnTagExamination)
                        {
                            if (FindObjectOfType<HDK_InventoryManager>().freeSlots > 0)
                            {
                                if (OnTagAmmo)
                                {
                                    Destroy(hit.transform.gameObject);
                                    GetComponent<AudioSource>().clip = ItemPickup[Random.Range(0, ItemPickup.Length)];
                                    GetComponent<AudioSource>().volume = pickupVolume;
                                    GetComponent<AudioSource>().Play();
                                    hit.transform.gameObject.GetComponent<HDK_InventoryItem>().AddItem();
                                    Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("ItemPicked"));
                                    examinationLight.enabled = false;
                                }
                                else if (OnTagFlashlight)
                                {
                                    Destroy(hit.transform.gameObject);
                                    GetComponent<AudioSource>().clip = ItemPickup[Random.Range(0, ItemPickup.Length)];
                                    GetComponent<AudioSource>().volume = pickupVolume;
                                    GetComponent<AudioSource>().Play();
                                    hit.transform.gameObject.GetComponent<HDK_InventoryItem>().AddItem();
                                    Player.GetComponent<HDK_Flashlight>().SendMessage("HasFlashlight");
                                    Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("ItemPicked"));
                                    examinationLight.enabled = false;
                                }
                                else if (OnTagFlashlightBattery)
                                {
                                    Destroy(hit.transform.gameObject);
                                    GetComponent<AudioSource>().clip = ItemPickup[Random.Range(0, ItemPickup.Length)];
                                    GetComponent<AudioSource>().volume = pickupVolume;
                                    GetComponent<AudioSource>().Play();
                                    hit.transform.gameObject.GetComponent<HDK_InventoryItem>().AddItem();
                                    Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("ItemPicked"));
                                    examinationLight.enabled = false;
                                }
                                else if (OnTagFood)
                                {
                                    Destroy(hit.transform.gameObject);
                                    GetComponent<AudioSource>().clip = ItemPickup[Random.Range(0, ItemPickup.Length)];
                                    GetComponent<AudioSource>().volume = pickupVolume;
                                    GetComponent<AudioSource>().Play();
                                    hit.transform.gameObject.GetComponent<HDK_InventoryItem>().AddItem();
                                    Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("ItemPicked"));
                                    examinationLight.enabled = false;
                                }
                                else if (OnTagKey)
                                {
                                    Destroy(hit.transform.gameObject);
                                    GetComponent<AudioSource>().clip = ItemPickup[Random.Range(0, ItemPickup.Length)];
                                    GetComponent<AudioSource>().volume = pickupVolume;
                                    GetComponent<AudioSource>().Play();
                                    hit.transform.gameObject.GetComponent<HDK_InventoryItem>().AddItem();
                                    Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("ItemPicked"));
                                    examinationLight.enabled = false;
                                }
                                else if (OnTagTelecamera)
                                {
                                    Destroy(hit.transform.gameObject);
                                    GetComponent<AudioSource>().clip = ItemPickup[Random.Range(0, ItemPickup.Length)];
                                    GetComponent<AudioSource>().volume = pickupVolume;
                                    GetComponent<AudioSource>().Play();
                                    hit.transform.gameObject.GetComponent<HDK_InventoryItem>().AddItem();
                                    Player.GetComponent<HDK_DigitalCamera>().HasCamera = true;
                                    Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("ItemPicked"));
                                    examinationLight.enabled = false;
                                }
                                else if (OnTagWeapon)
                                {
                                    Destroy(hit.transform.gameObject);
                                    GetComponent<AudioSource>().clip = ItemPickup[Random.Range(0, ItemPickup.Length)];
                                    GetComponent<AudioSource>().volume = pickupVolume;
                                    GetComponent<AudioSource>().Play();
                                    hit.transform.gameObject.GetComponent<HDK_InventoryItem>().AddItem();
                                    Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("ItemPicked"));
                                    examinationLight.enabled = false;
                                }
                                if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                                {
                                    Player.GetComponent<HDK_DigitalCamera>().CameraUI.GetComponent<CanvasGroup>().alpha = 1;
                                    if (Player.GetComponent<HDK_DigitalCamera>().broken)
                                    {
                                        Player.GetComponent<HDK_DigitalCamera>().camera_effect.enabled = true;
                                        Player.GetComponent<HDK_DigitalCamera>().brokenGUI.GetComponent<CanvasGroup>().alpha = 1;
                                    }
                                }
                                if (hasHBob)
                                {
                                    GetComponentInParent<HeadBobController>().enabled = true;
                                }
                                if (hasPeak)
                                {
                                    GetComponentInParent<HDK_Leaning>().enabled = true;
                                }
                                if (Player.GetComponent<HDK_Stamina>() != null)
                                {
                                    Player.GetComponent<HDK_Stamina>().Busy(false);
                                }
                                examineEyeIcon.GetComponent<HDK_UIFade>().FadeOut();
                                ExaminingObject = false;
                                ShowExaminingInfoGui = false;
                                Player.GetComponent<FirstPersonController>().enabled = true;
                                Player.GetComponentInChildren<HDK_ExamineRotation>().enabled = false;
                                Player.GetComponentInChildren<HDK_ExamineRotation>().target = null;
                                Cursor.visible = false;
                                Cursor.lockState = CursorLockMode.Locked;
                                if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera && !Player.GetComponent<HDK_DigitalCamera>().broken)
                                {
                                    Player.GetComponentInChildren<HDK_DigitalCamera>().canZoom = true;
                                }
                                if (!Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                                {
                                    Player.GetComponent<HDK_MouseZoom>().canZoom = true;
                                }
                            }
                            else
                            {
                                Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("InventoryFull"));
                                GetComponent<AudioSource>().clip = InventoryFull;
                                GetComponent<AudioSource>().volume = 1f;
                                GetComponent<AudioSource>().Play();
                            }
                        }
                        else
                        {
                            GetComponent<AudioSource>().clip = InventoryFull;
                            GetComponent<AudioSource>().volume = 1f;
                            GetComponent<AudioSource>().Play();
                            Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("CantTakeObj"));
                        }
                    }
                }
            }
            else if (OnTagPaper)
            {
                if (!ExaminingObject)
                {
                    if (Input.GetButtonDown("Interact"))
                    {
                        if (targetPaperNote.GetComponent<CanvasGroup>().alpha == 0)
                        {
                            if (GetComponentInParent<HeadBobController>() != null)
                            {
                                hasHBob = true;
                                GetComponentInParent<HeadBobController>().enabled = false;
                            }
                            else
                            {
                                hasHBob = false;
                            }
                            if (GetComponentInParent<HDK_Leaning>() != null)
                            {
                                hasPeak = true;
                                GetComponentInParent<HDK_Leaning>().enabled = false;
                            }
                            else
                            {
                                hasPeak = false;
                            }
                            if (Player.GetComponent<HDK_Stamina>() != null)
                            {
                                Player.GetComponent<HDK_Stamina>().Busy(true);
                            }
                            if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                            {
                                Player.GetComponent<HDK_DigitalCamera>().CameraUI.GetComponent<CanvasGroup>().alpha = 0;
                                if (Player.GetComponent<HDK_DigitalCamera>().broken)
                                {
                                    Player.GetComponent<HDK_DigitalCamera>().brokenGUI.GetComponent<CanvasGroup>().alpha = 0;
                                    Player.GetComponent<HDK_DigitalCamera>().camera_effect.enabled = false;
                                }
                            }
                            ReadingPaper = true;
                            Player.GetComponent<HDK_MouseZoom>().SendMessage("ZoomOut");
                            Player.GetComponentInChildren<HDK_DigitalCamera>().canZoom = false;
                            Cursor.visible = true;
                            Cursor.lockState = CursorLockMode.None;
                            targetPaperNote.GetComponent<HDK_UIFade>().FadeIn();
                            Player.GetComponent<FirstPersonController>().enabled = false;
                            GetComponent<AudioSource>().clip = PaperInteract[Random.Range(0, PaperInteract.Length)];
                            GetComponent<AudioSource>().volume = pickupVolume;
                            GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            if (!Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                            {
                                Player.GetComponent<HDK_MouseZoom>().enabled = true;
                                Player.GetComponent<HDK_MouseZoom>().canZoom = true;
                            }
                            if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera && !Player.GetComponent<HDK_DigitalCamera>().broken)
                            {
                                Player.GetComponentInChildren<HDK_DigitalCamera>().canZoom = true;
                            }
                            if (hasHBob)
                            {
                                GetComponentInParent<HeadBobController>().enabled = true;
                            }
                            if (hasPeak)
                            {
                                GetComponentInParent<HDK_Leaning>().enabled = true;
                            }
                            if (Player.GetComponent<HDK_Stamina>() != null)
                            {
                                Player.GetComponent<HDK_Stamina>().Busy(false);
                            }
                            ReadingPaper = false;
                            Player.GetComponent<HDK_DigitalCamera>().CameraUI.GetComponent<CanvasGroup>().alpha = 1;
                            if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                            {
                                if (Player.GetComponent<HDK_DigitalCamera>().broken)
                                {
                                    Player.GetComponent<HDK_DigitalCamera>().camera_effect.enabled = true;
                                    Player.GetComponent<HDK_DigitalCamera>().brokenGUI.GetComponent<CanvasGroup>().alpha = 1;
                                }
                            }
                            targetPaperNote.GetComponent<HDK_UIFade>().FadeOut();
                            Player.GetComponent<FirstPersonController>().enabled = true;
                            GetComponent<AudioSource>().clip = PaperInteract[Random.Range(0, PaperInteract.Length)];
                            GetComponent<AudioSource>().volume = pickupVolume;
                            GetComponent<AudioSource>().Play();
                            Cursor.visible = false;
                            Cursor.lockState = CursorLockMode.Locked;
                        }
                    }
                }
            }
            else if (OnTagLamp)
            {
                if (!ExaminingObject)
                {
                    if (Input.GetButtonDown("Interact"))
                    {
                        if (RaycastedLamp.GetComponent<HDK_SwitchableLamp>().isOn)
                        {
                            RaycastedLamp.SendMessage("SwitchOff");
                        }
                        else
                        {
                            RaycastedLamp.SendMessage("SwitchOn");
                        }
                    }
                }
            }
            else if (OnTagPlayAudio)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (!raycasted_obj.GetComponent<HDK_PlayableAudio>().isPlaying)
                    {
                        raycasted_obj.SendMessage("PlaySound");
                    }
                }
            }
            else if (OnTagSecurityCam)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (!UsingSecurityCam)
                    {
                        UsingSecurityCam = true;
                        Player.GetComponent<FirstPersonController>().enabled = false;
                        Player.GetComponent<HDK_MouseLook>().enabled = false;
                        GameObject.Find("MouseLook").GetComponent<HDK_MouseLook>().enabled = false;
                        raycasted_obj.GetComponent<HDK_SecurityMonitor>().StartCam();
                        if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                        {
                            Player.GetComponent<HDK_DigitalCamera>().CameraUI.GetComponent<CanvasGroup>().alpha = 0;
                            if (Player.GetComponent<HDK_DigitalCamera>().broken)
                            {
                                Player.GetComponent<HDK_DigitalCamera>().brokenGUI.GetComponent<CanvasGroup>().alpha = 0;
                                Player.GetComponent<HDK_DigitalCamera>().camera_effect.enabled = false;
                            }
                        }
                        Player.GetComponent<HDK_MouseZoom>().SendMessage("ZoomOut");
                        Player.GetComponent<HDK_DigitalCamera>().canZoom = false;

                    }
                    else if (UsingSecurityCam)
                    {
                        UsingSecurityCam = false;
                        Player.GetComponent<FirstPersonController>().enabled = true;
                        Player.GetComponent<HDK_MouseLook>().enabled = true;
                        GameObject.Find("MouseLook").GetComponent<HDK_MouseLook>().enabled = true;
                        raycasted_obj.GetComponent<HDK_SecurityMonitor>().EndCam();
                        if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                        {
                            Player.GetComponent<HDK_DigitalCamera>().CameraUI.GetComponent<CanvasGroup>().alpha = 1;
                            if (Player.GetComponent<HDK_DigitalCamera>().broken)
                            {
                                Player.GetComponent<HDK_DigitalCamera>().camera_effect.enabled = true;
                                Player.GetComponent<HDK_DigitalCamera>().brokenGUI.GetComponent<CanvasGroup>().alpha = 1;
                            }
                        }
                        if (!Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
                        {
                            Player.GetComponent<HDK_MouseZoom>().enabled = true;
                            Player.GetComponent<HDK_MouseZoom>().canZoom = true;
                        }
                        if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera && !Player.GetComponent<HDK_DigitalCamera>().broken)
                        {
                            Player.GetComponent<HDK_DigitalCamera>().canZoom = true;
                        }
                    }
                }
            }
            else if (OnTagDoor)
            {
                if (raycasted_obj.GetComponent<HDK_DraggableDoor>())
                {
                    furnitureRaycasted = "DraggableDoor";
                    //Already scripted
                }
                else if (raycasted_obj.GetComponent<HDK_NormalDoor>())
                {
                    furnitureRaycasted = "NormalDoor";
                    if (Input.GetButton("Interact"))
                    {
                        if (raycasted_obj.GetComponent<HDK_NormalDoor>().TypeOfSafe == SafeMode.Open)
                        {
                            if (!raycasted_obj.GetComponent<HDK_NormalDoor>().performing)
                            {
                                raycasted_obj.SendMessage("PerformAction");
                            }
                        }
                        else if (raycasted_obj.GetComponent<HDK_NormalDoor>().TypeOfSafe == SafeMode.Locked)
                        {
                            Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("DoorLocked"));
                            raycasted_obj.SendMessage("PlayAudioClip", DoorGrab.Locked);
                        }
                        else if (raycasted_obj.GetComponent<HDK_NormalDoor>().TypeOfSafe == SafeMode.Jammed)
                        {
                            Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("DoorJammed"));
                            raycasted_obj.SendMessage("PlayAudioClip", DoorGrab.Locked);
                        }
                    }
                }
                else if (raycasted_obj.GetComponent<HDK_Drawer>())
                {
                    furnitureRaycasted = "Drawer";
                    if (Input.GetButtonDown("Interact"))
                    {
                        if (raycasted_obj.GetComponent<HDK_Drawer>().TypeOfSafe == SafeMode.Open)
                        {
                            if (!raycasted_obj.GetComponent<HDK_Drawer>().performing)
                            {
                                raycasted_obj.SendMessage("PerformAction");
                            }
                        }
                        else if (raycasted_obj.GetComponent<HDK_Drawer>().TypeOfSafe == SafeMode.Locked)
                        {
                            Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("DrawerLocked"));
                            raycasted_obj.SendMessage("PlayAudioClip", DoorGrab.Locked);
                        }
                        else if (raycasted_obj.GetComponent<HDK_Drawer>().TypeOfSafe == SafeMode.Jammed)
                        {
                            Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("DrawerJammed"));
                            raycasted_obj.SendMessage("PlayAudioClip", DoorGrab.Locked);
                        }
                    }
                }
            }
        }

        public void CloseCam()
        {
            UsingSecurityCam = false;
            Player.GetComponent<FirstPersonController>().enabled = true;
            Player.GetComponent<HDK_MouseLook>().enabled = true;
            GameObject.Find("MouseLook").GetComponent<HDK_MouseLook>().enabled = true;
            raycasted_obj.GetComponent<HDK_SecurityMonitor>().EndCam();
            if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
            {
                Player.GetComponent<HDK_DigitalCamera>().CameraUI.GetComponent<CanvasGroup>().alpha = 1;
                if (Player.GetComponent<HDK_DigitalCamera>().broken)
                {
                    Player.GetComponent<HDK_DigitalCamera>().camera_effect.enabled = true;
                    Player.GetComponent<HDK_DigitalCamera>().brokenGUI.GetComponent<CanvasGroup>().alpha = 1;
                }
            }
            if (!Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
            {
                Player.GetComponent<HDK_MouseZoom>().enabled = true;
                Player.GetComponent<HDK_MouseZoom>().canZoom = true;
            }
            if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera && !Player.GetComponent<HDK_DigitalCamera>().broken)
            {
                Player.GetComponentInChildren<HDK_DigitalCamera>().canZoom = true;
            }
        }

        public void ClosePaper()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (!Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
            {
                Player.GetComponent<HDK_MouseZoom>().enabled = true;
                Player.GetComponent<HDK_MouseZoom>().canZoom = true;
            }
            if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera && !Player.GetComponent<HDK_DigitalCamera>().broken)
            {
                Player.GetComponentInChildren<HDK_DigitalCamera>().canZoom = true;
            }
            if (hasHBob)
            {
                GetComponentInParent<HeadBobController>().enabled = true;
            }
            if (hasPeak)
            {
                GetComponentInParent<HDK_Leaning>().enabled = true;
            }
            if (Player.GetComponent<HDK_Stamina>() != null)
            {
                Player.GetComponent<HDK_Stamina>().Busy(false);
            }
            ReadingPaper = false;
            Player.GetComponent<HDK_DigitalCamera>().CameraUI.GetComponent<CanvasGroup>().alpha = 1;
            if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
            {
                if (Player.GetComponent<HDK_DigitalCamera>().broken)
                {
                    Player.GetComponent<HDK_DigitalCamera>().camera_effect.enabled = true;
                    Player.GetComponent<HDK_DigitalCamera>().brokenGUI.GetComponent<CanvasGroup>().alpha = 1;
                }
            }
            targetPaperNote.GetComponent<HDK_UIFade>().FadeOut();
            Player.GetComponent<FirstPersonController>().enabled = true;
            GetComponent<AudioSource>().clip = PaperInteract[Random.Range(0, PaperInteract.Length)];
            GetComponent<AudioSource>().volume = pickupVolume;
            GetComponent<AudioSource>().Play();
        }

        public void PutDownObject()
        {
            raycasted_obj.SendMessage("RestorePos");
            if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
            {
                Player.GetComponent<HDK_DigitalCamera>().CameraUI.GetComponent<CanvasGroup>().alpha = 1;
                if (Player.GetComponent<HDK_DigitalCamera>().broken)
                {
                    Player.GetComponent<HDK_DigitalCamera>().camera_effect.enabled = true;
                    Player.GetComponent<HDK_DigitalCamera>().brokenGUI.GetComponent<CanvasGroup>().alpha = 1;
                }
            }
            if (hasHBob)
            {
                GetComponentInParent<HeadBobController>().enabled = true;
            }
            if (hasPeak)
            {
                GetComponentInParent<HDK_Leaning>().enabled = true;
            }
            if (Player.GetComponent<HDK_Stamina>() != null)
            {
                Player.GetComponent<HDK_Stamina>().Busy(false);
            }
            examineEyeIcon.GetComponent<HDK_UIFade>().FadeOut();
            ExaminingObject = false;
            ShowExaminingInfoGui = false;
            Player.GetComponent<FirstPersonController>().enabled = true;
            Player.GetComponentInChildren<HDK_ExamineRotation>().enabled = false;
            Player.GetComponentInChildren<HDK_ExamineRotation>().target = null;
            examinationLight.enabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (Player.GetComponent<HDK_DigitalCamera>().UsingCamera && !Player.GetComponent<HDK_DigitalCamera>().broken)
            {
                Player.GetComponentInChildren<HDK_DigitalCamera>().canZoom = true;
            }
            if (!Player.GetComponent<HDK_DigitalCamera>().UsingCamera)
            {
                Player.GetComponent<HDK_MouseZoom>().canZoom = true;
            }
        }

        private void holdObject()
        {
            Ray playerAim = playerCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 nextPos = playerCam.transform.position + playerAim.direction * distance;
            Vector3 currPos = objectHeld.transform.position;

            objectHeld.GetComponent<Rigidbody>().velocity = (nextPos - currPos) * 10;
            if (Vector3.Distance(objectHeld.transform.position, playerCam.transform.position) > maxDistanceGrab)
            {
                DropObject();
            }
        }

        private void DropObject()
        {
            isObjectHeld = false;
            tryPickupObject = false;
            objectHeld.GetComponent<Rigidbody>().useGravity = true;
            objectHeld.GetComponent<Rigidbody>().freezeRotation = false;
            objectHeld = null;
        }

        private void tryPickObject()
        {
            Ray playerAim = playerCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(playerAim, out hit, distance))
            {
                objectHeld = hit.collider.gameObject;
                if (hit.collider.tag == DoorTag && tryPickupObject)
                {
                    if (objectHeld.GetComponent<HDK_DraggableDoor>().TypeOfSafe == SafeMode.Open)
                    {
                        isObjectHeld = true;
                        objectHeld.GetComponent<Rigidbody>().useGravity = true;
                        objectHeld.GetComponent<Rigidbody>().freezeRotation = false;
                        distance = DoorGrab.DoorPickupRange;
                        distance = DoorGrab.DoorDistance;
                        maxDistanceGrab = DoorGrab.DoorMaxGrab;
                    }
                    else if (objectHeld.GetComponent<HDK_DraggableDoor>().TypeOfSafe == SafeMode.Jammed)
                    {
                        Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("DoorJammed"));
                        objectHeld.GetComponent<HDK_DraggableDoor>().PlayAudioClip(DoorGrab.Locked);
                    }
                    else if (objectHeld.GetComponent<HDK_DraggableDoor>().TypeOfSafe == SafeMode.Locked)
                    {
                        Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("DoorLocked"));
                        objectHeld.GetComponent<HDK_DraggableDoor>().PlayAudioClip(DoorGrab.Locked);
                    }   
                }
            }
        }
    }
}