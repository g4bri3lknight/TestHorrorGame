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

    private DataBaseManager dbManager;

    // Use this for initialization
    void Start()
    {
        globalLight = GetComponent<Light>();
        globalLight.enabled = false;

        Assert.IsNotNull(inventory);
        inventory.SetActive(false);

        Assert.IsNotNull(console);
        console.SetActive(false);

        //TEST CONNECTION
        //dbManager = GetComponent<DataBaseManager>();
        // dbManager.DBConnect();
    }
	
    // Update is called once per frame
    void Update()
    {
		
    }
}
