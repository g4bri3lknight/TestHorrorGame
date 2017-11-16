using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroyNotify : MonoBehaviour
{
    public void OnDestroy()
    {
        SaveLoadManager.AddObjectToDestroy(this.gameObject.name);
    }

    public void OnDestroy(GameObject obj)
    {
        SaveLoadManager.AddObjectToDestroy(obj.name);
    }
}
