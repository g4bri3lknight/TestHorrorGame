using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteraction : MonoBehaviour
{

    [Header("Raycast")]
    public float distance = 2.0f;
    public LayerMask layerMaskInteract;
    public Color RayGizmoColor;
    RaycastHit hit;

    //The raycasted item
    public GameObject raycasted_obj;

    GameObject Player;


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
        // Debug.DrawRay(transform.position, direction * distance, RayGizmoColor);

        if (Input.GetButtonDown("Interact"))
        {
            if (Physics.Raycast(position, direction, out hit, distance, layerMaskInteract.value))
            {
            
                if (hit.transform.gameObject.GetComponent<HDK_InventoryItem>() && hit.transform.gameObject.tag.Equals("Chest"))
                {
                    raycasted_obj = hit.transform.gameObject;

                    bool isLocked = raycasted_obj.GetComponent<ChestScript>().isLocked;

                    if (raycasted_obj.GetComponent<ChestScript>().TypeOfSafe.Equals(SafeMode.Jammed))
                    {
                        Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", "E' rotto e non puo' essere aperto");
                        raycasted_obj.GetComponent<ChestScript>().SendMessage("PerformJammedAudio", SendMessageOptions.RequireReceiver);
                    }

                    if (raycasted_obj.GetComponent<ChestScript>().TypeOfSafe.Equals(SafeMode.Locked) && isLocked)
                    {
                        Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", "E' chiuso! Mi serve una chiave per aprirlo");
                        raycasted_obj.GetComponent<ChestScript>().SendMessage("PerformLockedAudio", SendMessageOptions.RequireReceiver);
                    }

                    if (raycasted_obj.GetComponent<ChestScript>().TypeOfSafe.Equals(SafeMode.Locked) && !isLocked)
                    {
                        raycasted_obj.GetComponent<ChestScript>().SendMessage("PerformOpenAnimation", SendMessageOptions.RequireReceiver);
                    }

                    if (raycasted_obj.GetComponent<ChestScript>().TypeOfSafe.Equals(SafeMode.Open))
                    {
                        Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", "E' aperto!");
                    }
                }
            }
        }
    }
}
