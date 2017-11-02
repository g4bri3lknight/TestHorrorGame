using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroyNotify : MonoBehaviour
{
    private ConsoleCommands consoleCommands;

    void Start()
    {
        consoleCommands = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ConsoleCommands>();
    }

    void OnDestroy()
    {
        consoleCommands.AddObjectToDestroy(this.gameObject.name);
    }
}
