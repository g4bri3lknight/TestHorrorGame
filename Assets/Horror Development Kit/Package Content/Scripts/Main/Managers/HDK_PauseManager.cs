//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

public class HDK_PauseManager : MonoBehaviour
{
    public static bool GamePaused;

    [Header("ConsoleCommands")]
    public ConsoleCommands consoleCommands;

    [Header("Main Variables")]
    public GameObject PauseMenu;
    public GameObject OptionsPanel;
    public Text loadSaveText;
    GameObject Player;
    public GameObject[] ToDisableGUI;


    [Header("Mouse SFX")]
    public AudioClip mouseHover;
    public AudioClip mouseClick;
    public AudioClip openCloseSound;
    AudioSource sourceAudio;

    public float mouseVolume;
    GameObject LoadingPrefab;

    void Start()
    {
        GamePaused = false;
        Player = GameObject.Find("Player");
        sourceAudio = GetComponent<AudioSource>();
        LoadingPrefab = GameObject.Find("Player").GetComponent<GameSceneManager>()._loadingScreen;
    }

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void PlayHover()
    {
        sourceAudio.PlayOneShot(mouseHover, mouseVolume);
    }

    public void PlayClick()
    {
        sourceAudio.PlayOneShot(mouseClick, mouseVolume);
    }

    public void LoadScene(string sceneToLoad)
    {
        HDK_LoadingScreen.LoadScene(sceneToLoad, LoadingPrefab);
    }

    public void UnPause()
    {
        Player.GetComponentInChildren<BlurOptimized>().enabled = false;
        Player.GetComponentInChildren<HDK_RaycastManager>().enabled = true;
        Player.GetComponent<HDK_MouseZoom>().canZoom = true;
        Player.GetComponentInChildren<HeadBobController>().enabled = true;
        Player.GetComponentInChildren<SwayWeapon>().enabled = true;
        Player.GetComponent<HDK_Stamina>().Busy(false);
        GamePaused = false;
        PauseMenu.SetActive(false);
        Player.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        sourceAudio.clip = openCloseSound;
        sourceAudio.Play();
        foreach (GameObject obj in ToDisableGUI)
        {
            obj.SetActive(true);
        }
        //Time.timeScale = 1;
    }

    public void DoPause()
    {
        Player.GetComponentInChildren<BlurOptimized>().enabled = true;
        Player.GetComponentInChildren<HDK_RaycastManager>().enabled = false;
        Player.GetComponent<HDK_MouseZoom>().ZoomOut();
        Player.GetComponentInChildren<SwayWeapon>().enabled = false;
        Player.GetComponentInChildren<HeadBobController>().enabled = false;
        Player.GetComponent<HDK_Stamina>().Busy(true);
        Player.GetComponent<FirstPersonController>().enabled = false;
        GamePaused = true;
        PauseMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        sourceAudio.clip = openCloseSound;
        sourceAudio.Play();
        foreach (GameObject obj in ToDisableGUI)
        {
            obj.SetActive(false);
        }
        //Time.timeScale = 0;
    }

    void Update()
    {
        bool examining = HDK_RaycastManager.ExaminingObject;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool inventory = HDK_InventoryManager.inventoryOpen;
        
        if (!GamePaused && !examining && !reading && !security && !inventory)
        {
            if (Input.GetButtonDown("PauseMenu"))
            {
                DoPause();
            }
        }
        else if (examining)
        {
            if (Input.GetButtonDown("PauseMenu"))
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
                DoPause();
            }
        }
        else if (reading)
        {
            if (Input.GetButtonDown("PauseMenu"))
            {
                Player.GetComponentInChildren<HDK_RaycastManager>().ClosePaper();
                DoPause();
            }
        }
        else if (security)
        {
            if (Input.GetButtonDown("PauseMenu"))
            {
                Player.GetComponentInChildren<HDK_RaycastManager>().CloseCam();
                DoPause();
            }
        }
        else if (inventory)
        {
            if (Input.GetButtonDown("PauseMenu"))
            {
                FindObjectOfType<HDK_InventoryManager>().CloseInventory();
                DoPause();
            }
        }
        /* else if (GamePaused) //disabilitato perche' buggato!!
        {
            if (Input.GetButtonDown("PauseMenu"))
            {
                UnPause();
            }
        }*/
        
        if (OptionsPanel.activeInHierarchy == true)
        {
            //Texture
            if (QualitySettings.masterTextureLimit == 0)
            {
                GameObject.Find("TextureQuality").GetComponent<Dropdown>().value = 2;
            }

            if (QualitySettings.masterTextureLimit == 1)
            {
                GameObject.Find("TextureQuality").GetComponent<Dropdown>().value = 1;
            }

            if (QualitySettings.masterTextureLimit == 2)
            {
                GameObject.Find("TextureQuality").GetComponent<Dropdown>().value = 0;
            }
            //Anti aliasing
            if (QualitySettings.antiAliasing == 0)
            {
                GameObject.Find("AA").GetComponent<Dropdown>().value = 0;
            }

            if (QualitySettings.antiAliasing == 2)
            {
                GameObject.Find("AA").GetComponent<Dropdown>().value = 1;
            }

            if (QualitySettings.antiAliasing == 4)
            {
                GameObject.Find("AA").GetComponent<Dropdown>().value = 2;
            }

            if (QualitySettings.antiAliasing == 8)
            {
                GameObject.Find("AA").GetComponent<Dropdown>().value = 3;
            }
            //Anisotropic
            if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable)
            {
                GameObject.Find("AS").GetComponent<Dropdown>().value = 0;
            }

            if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable)
            {
                GameObject.Find("AS").GetComponent<Dropdown>().value = 1;
            }

            if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
            {
                GameObject.Find("AS").GetComponent<Dropdown>().value = 2;
            }
            //Geometry blend weights
            if (QualitySettings.blendWeights == BlendWeights.OneBone)
            {
                GameObject.Find("GeometryLevel").GetComponent<Dropdown>().value = 0;
            }
            if (QualitySettings.blendWeights == BlendWeights.TwoBones)
            {
                GameObject.Find("GeometryLevel").GetComponent<Dropdown>().value = 1;
            }
            if (QualitySettings.blendWeights == BlendWeights.FourBones)
            {
                GameObject.Find("GeometryLevel").GetComponent<Dropdown>().value = 2;
            }
            //Shadow cascades
            if (QualitySettings.shadowCascades == 0)
            {
                GameObject.Find("ShadowsCascades").GetComponent<Dropdown>().value = 0;
            }
            if (QualitySettings.shadowCascades == 2)
            {
                GameObject.Find("ShadowsCascades").GetComponent<Dropdown>().value = 1;
            }
            if (QualitySettings.shadowCascades == 4)
            {
                GameObject.Find("ShadowsCascades").GetComponent<Dropdown>().value = 2;
            }
            //Vsync option
            if (QualitySettings.vSyncCount == 0)
            {
                GameObject.Find("VSyncToogle").GetComponent<Toggle>().isOn = false;
            }
            if (QualitySettings.vSyncCount == 1)
            {
                GameObject.Find("VSyncToogle").GetComponent<Toggle>().isOn = true;
            }
            //
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateVolume(float v)
    {
        AudioListener.volume = v;
    }

    public void MSAALevel(int a)
    {
        a = GameObject.Find("AA").GetComponent<Dropdown>().value;
        if (a == 0)
        {
            QualitySettings.antiAliasing = 0;
        }
        if (a == 1)
        {
            QualitySettings.antiAliasing = 2;
        }
        if (a == 2)
        {
            QualitySettings.antiAliasing = 4;
        }
        if (a == 3)
        {
            QualitySettings.antiAliasing = 8;
        }
    }

    public void TextureQuality(int te)
    {
        te = GameObject.Find("TextureQuality").GetComponent<Dropdown>().value;
        if (te == 0)
        {
            QualitySettings.masterTextureLimit = 2;
        }
        if (te == 1)
        {
            QualitySettings.masterTextureLimit = 1;
        }
        if (te == 2)
        {
            QualitySettings.masterTextureLimit = 0;
        }
    }

    public void UpdateAnisotropic(int a)
    {
        a = GameObject.Find("AS").GetComponent<Dropdown>().value;
        if (a == 0)
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        }
        if (a == 1)
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        }
        if (a == 2)
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        }
    }

    public void BlendWeight(int bw)
    {
        bw = GameObject.Find("GeometryLevel").GetComponent<Dropdown>().value;
        if (bw == 0)
        {
            QualitySettings.blendWeights = BlendWeights.OneBone;
        }
        if (bw == 1)
        {
            QualitySettings.blendWeights = BlendWeights.TwoBones;
        }
        if (bw == 2)
        {
            QualitySettings.blendWeights = BlendWeights.FourBones;
        }
    }

    public void VSync(bool vs)
    {
        vs = GameObject.Find("VSyncToogle").GetComponent<Toggle>().isOn;
        if (vs == true)
        {
            QualitySettings.vSyncCount = 1;
        }
        if (vs == false)
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void ShadowsCascades(int s)
    {
        s = GameObject.Find("ShadowsCascades").GetComponent<Dropdown>().value;
        if (s == 0)
        {
            QualitySettings.shadowCascades = 0;
        }
        if (s == 1)
        {
            QualitySettings.shadowCascades = 2;
        }
        if (s == 2)
        {
            QualitySettings.shadowCascades = 4;
        }
    }


    public void SaveGame()
    {
        consoleCommands.Save();
        StartCoroutine(ShowText("Salvataggio effettuato"));
    }

    public void LoadGame()
    {
        consoleCommands.Load();
        StartCoroutine(ShowText("Caricamento effettuato"));
    }

    IEnumerator ShowText(string text)
    {
        loadSaveText.text = text;
        yield return new WaitForSeconds(2f);
        loadSaveText.text = "";
    }
}