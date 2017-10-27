using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

using System;
using UnityEditor;

[System.Serializable]
public class ConsoleManager : MonoBehaviour
{
    [Header("Parameters")]
    public GameObject consoleGUI;

    private GameObject Player;
  
    private Dictionary<string,Command> mapCommand = new Dictionary<string,Command>();

    private List<string> historyList = new List<string>();
    private int index = 0;



    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        loadCommandList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote) && Input.GetKey(KeyCode.LeftControl))
        {
            if (consoleGUI)
            {
                if (!consoleGUI.activeSelf)
                {
                    FreezeAll();
                    ShowConsole();
                }
                else
                {
                    StopAllCoroutines();
                    RestoreAll();
                    HideConsole();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            try
            {

                if (index == historyList.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }

                consoleGUI.GetComponentInChildren<InputField>().text = historyList[index];
            }
            catch
            {
                index = 0;
                Debug.Log("index in history not found!");
            }

        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            try
            {
                if (index == 0)
                {
                    index = historyList.Count - 1;
                }
                else
                {
                    index--;
                }

                consoleGUI.GetComponentInChildren<InputField>().text = historyList[index];
            }
            catch
            {
                index = 0;
                Debug.Log("index in history not found!");
            }
        }
    }


    private void loadCommandList()
    {
        //Si puo'caricare da db o file
        Command cmd = new Command("create", "create an object", 2, "Usage: create [object name] [0<quantity<30]", true, "CreateObject");
        mapCommand.Add(cmd.CommandName, cmd);

        cmd = new Command("saveinventory", "save the inventory", 0, "Usage: saveinventory", true, "SaveInventory");
        mapCommand.Add(cmd.CommandName, cmd);

        cmd = new Command("loadinventory", "load the inventory", 0, "Usage: loadinventory", true, "LoadInventory");
        mapCommand.Add(cmd.CommandName, cmd);
    }

    private Command findCommand(string commandName)
    {
        Command command = null;

        mapCommand.TryGetValue(commandName, out command);
       
        return command;
    }


    public void Validate()
    {
        StartCoroutine(CheckText());
    }

    private IEnumerator CheckText()
    { 
        string commandName = "";
        string text = "";
        string originalCommandLine = consoleGUI.GetComponentInChildren<InputField>().text;

        if (originalCommandLine.Trim() == "")
        {
            yield return new WaitForSeconds(0.2f);
        }

        string[] listOfString = consoleGUI.GetComponentInChildren<InputField>().text.Split(' ');

        consoleGUI.GetComponentInChildren<InputField>().text = "";
        consoleGUI.GetComponentInChildren<InputField>().ActivateInputField();

        commandName = listOfString[0].ToLower();

        ConsoleCommands comandiConsole = GetComponent<ConsoleCommands>();

        Command command = findCommand(commandName);

        if (command != null)
        {
            try
            {
                comandiConsole = GetComponent<ConsoleCommands>();

                comandiConsole.CommandLine = listOfString;
                comandiConsole.Command = command;

                comandiConsole.SendMessage(command.MethodToCall, SendMessageOptions.RequireReceiver);

                comandiConsole.AddText("Info: " + command.CommandName + " executed", "green");
                addToHistoryList(originalCommandLine);

                UpdateCanvas();
            }
            catch (Exception ex)
            {
                Debug.LogError("Errore in " + command.MethodToCall + ": " + ex);
            }
        }
        else
        {
            text = string.Format("<color=white>{0}</color>", commandName);

            comandiConsole.AddText("Error: command " + commandName + " not found!", "red");
            yield return new WaitForSeconds(0.2f);
        }
    }


    void UpdateCanvas()
    {
        Canvas.ForceUpdateCanvases();
        consoleGUI.GetComponentInChildren<ScrollRect>().verticalScrollbar.value = 0f;
        consoleGUI.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }


    void addToHistoryList(string command)
    {
        if (!historyList.Contains(command.ToLower()))
        {
            historyList.Add(command);
        }
    }

       
    void ShowConsole()
    {
        if (consoleGUI)
        {
            consoleGUI.SetActive(true);
            consoleGUI.GetComponentInChildren<InputField>().text = "";
            consoleGUI.GetComponentInChildren<InputField>().ActivateInputField();
        }
    }

    void HideConsole()
    {
        if (consoleGUI)
        {
            consoleGUI.SetActive(false);
        }
    }



    void FreezeAll()
    {       
        Player.GetComponentInChildren<BlurOptimized>().enabled = true;
        Player.GetComponentInChildren<HDK_RaycastManager>().enabled = false;
        Player.GetComponent<HDK_MouseZoom>().ZoomOut();
        Player.GetComponentInChildren<SwayWeapon>().enabled = false;
        Player.GetComponentInChildren<HeadBobController>().enabled = false;
        Player.GetComponent<HDK_Stamina>().Busy(true);
        Player.GetComponent<FirstPersonController>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void RestoreAll()
    {       
        Player.GetComponentInChildren<BlurOptimized>().enabled = false;
        Player.GetComponentInChildren<HDK_RaycastManager>().enabled = true;
        Player.GetComponent<HDK_MouseZoom>().ZoomOut();
        Player.GetComponentInChildren<SwayWeapon>().enabled = true;
        Player.GetComponentInChildren<HeadBobController>().enabled = true;
        Player.GetComponent<HDK_Stamina>().Busy(false);
        Player.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
