using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPackInteraction : MonoBehaviour
{

    [Header("Raycast")]
    public float distance = 2.0f;
    public LayerMask layerMaskInteract;
    public Color RayGizmoColor;
    RaycastHit hit;

    public GameObject inventoryLootGUI;
    public HDK_InventoryManager inventoryManager;

    //The raycasted item
    public GameObject raycasted_obj;

    GameObject Player;

    private bool canpickup;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player");
    }
	
    // Update is called once per frame
    void Update()
    {
        
        Vector3 position = transform.parent.position;
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, direction * distance, RayGizmoColor);

        if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (hit.transform.gameObject.GetComponent<HDK_InventoryItem>() && hit.transform.gameObject.tag == "BackPack")
                {
                    if (!canpickup)
                    {
                        raycasted_obj = hit.transform.gameObject;
                        string itemDescription = raycasted_obj.GetComponent<HDK_InventoryItem>().itemInfo;
                        Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", itemDescription);
                        canpickup = true;
                    }
                    else
                    {
                        StartCoroutine(PickUp());
                    }
                }
            }
        }
    }

    IEnumerator PickUp()
    {
        inventoryManager.SendMessage("OpenInventory");
        yield return new WaitForSecondsRealtime(1);
        inventoryLootGUI.GetComponent<Animator>().SetBool("enlarge", true);

        List<GameObject> listOfSlotsToAdd = raycasted_obj.GetComponent<ObjectToAdd>().listOfObjects;
   
        inventoryManager.inventorySlots.AddRange(listOfSlotsToAdd);

        Destroy(hit.transform.gameObject);
    }
}
