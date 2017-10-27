using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;
using UnityEngine;

public class MoveCrate : MonoBehaviour
{
    [Header("Parametri")]
    public GameObject player;
    public float minDistance;

    public GameObject defaultCursor;
    public GameObject dragCursor;
   

    private float distance;
    private Vector3 oldPosition;
    private float spostamento;
    private AudioSource audiosource;

    // Use this for initialization
    void Start()
    {
        GetComponent<DragRigidbody>().enabled = false;
        defaultCursor.SetActive(true);
        dragCursor.SetActive(false);
        oldPosition = this.transform.position;
        audiosource = GetComponent<AudioSource>();

    }

    void Update()
    {
        if (Input.GetMouseButton(0) && GetComponent<DragRigidbody>().enabled)
        {
            defaultCursor.SetActive(false);
            dragCursor.SetActive(true);

            if (!audiosource.isPlaying && GetComponent<Rigidbody>().velocity.magnitude >= 0.05)
            {              
                audiosource.Play();
                audiosource.SetScheduledEndTime(AudioSettings.dspTime + (2.1f - 0.1f));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            DisableGrab();
        }
    }

    public void  OnMouseOver()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);

        defaultCursor.SetActive(false);
        dragCursor.SetActive(true);

        if (distance <= minDistance)
        {          
            defaultCursor.SetActive(false);
            dragCursor.SetActive(true);
            GetComponent<DragRigidbody>().enabled = true;
        }
        else
        {
            DisableGrab();
        }
    }

    public void OnMouseExit()
    {
        defaultCursor.SetActive(true);
        dragCursor.SetActive(false);
    }

   
    private void DisableGrab()
    {
        oldPosition = transform.position;
        GetComponent<DragRigidbody>().enabled = false;

        if (!audiosource.isPlaying)
        {
            audiosource.Stop();
        }

        defaultCursor.SetActive(true);
        dragCursor.SetActive(false);
    }
}
