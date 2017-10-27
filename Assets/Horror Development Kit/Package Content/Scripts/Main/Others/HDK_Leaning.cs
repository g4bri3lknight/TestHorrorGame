using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class HDK_Leaning : MonoBehaviour {

    [Header("Camera Leaning Settings")]
    public bool isLeaning;
    bool canLean;
    public float smoothTime = 5f;
	public float angle = 10f;

	void Update()
    {
        bool examining = HDK_RaycastManager.ExaminingObject;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool paused = HDK_PauseManager.GamePaused;
        bool inventory = HDK_InventoryManager.inventoryOpen;

        if(!examining && !security && !reading && !paused && !inventory)
        {
            canLean = true;
        }
        else
        {
            canLean = false;
        }

        float rightpeek = Input.GetAxis("RightTrigger");
        float leftpeek = Input.GetAxis("LeftTrigger");

        if (canLean)
        {
            if (rightpeek != 0 || Input.GetKey(KeyCode.X))
            {
                isLeaning = true;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(0f, 0f, -angle)), Time.deltaTime * smoothTime);
            }

            else if (leftpeek != 0 || Input.GetKey(KeyCode.Z))
            {
                isLeaning = true;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(0f, 0f, angle)), Time.deltaTime * smoothTime);
            }
            else
            {
                isLeaning = false;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(0f, 0f, 0f)), Time.deltaTime * smoothTime);
            }
        }



       /* if (canLean && Input.GetKey(KeyCode.Z))
        {
            isLeaning = true;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(0f, 0f, angle)), Time.deltaTime * smoothTime);
        }
        else if (canLean && Input.GetKey(KeyCode.X))
        {
            isLeaning = true;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(0f, 0f, -angle)), Time.deltaTime * smoothTime);
        }
        else
        {
            isLeaning = false;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(0f, 0f, 0f)), Time.deltaTime * smoothTime);
        }*/
	}
}
