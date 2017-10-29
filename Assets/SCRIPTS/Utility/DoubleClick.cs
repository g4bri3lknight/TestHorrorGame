using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClick : MonoBehaviour,IPointerClickHandler
{

    private  HDK_InventorySlot item;

    public AudioClip EquipItemAudio;

    private  AudioSource audioItem;

    void Start()
    {
        audioItem = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2)
        {
            item = eventData.selectedObject.GetComponent<HDK_InventorySlot>();

            if (item && !item.Empty)
            {
                if (item.itemType == ItemType.Eatable || item.itemType == ItemType.Medikit)
                {
                    this.gameObject.SendMessageUpwards("UseItem", SendMessageOptions.RequireReceiver);
                }
                if (item.itemType == ItemType.Melee || item.itemType == ItemType.Firegun || item.itemType == ItemType.Key)
                {
                    audioItem.clip = EquipItemAudio;
                    audioItem.Play();
                    this.gameObject.SendMessageUpwards("EquipItem", SendMessageOptions.RequireReceiver);
                }  
            } 
        }  
    }
        
}
