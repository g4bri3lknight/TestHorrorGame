using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
[RequireComponent(typeof(Light))]
public class StartGame : MonoBehaviour
{

    private Light globalLight;

    [Header("Parametri")]
    [Tooltip("Oggetto Inventario presente nel Canvas")]
    public GameObject inventory;
    [Tooltip("Oggetto Console presente nel Canvas")]
    public GameObject console;

    private static StartGame instance;

    public static StartGame Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        // If no GameManager ever existed, we are it.
        if (instance == null)
            instance = this;
        // If one already exist, it's because it came from another level.
        else if (instance != this)
        {
            Destroy(this.transform.gameObject);
            return;
        }
    }
   
    // Use this for initialization
    void Start()
    {
        globalLight = GetComponent<Light>();
        globalLight.enabled = false;

        Assert.IsNotNull(inventory);
        inventory.SetActive(false);

        Assert.IsNotNull(console);
        console.SetActive(false);

        DontDestroyOnLoad(transform.gameObject);

    }
	
    // Update is called once per frame
    void Update()
    {
		
    }
}
