using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public static class SaveLoadManager
{
    public static List<string> listOfObjectsToDestroy = new List<string>();

    public static void AddObjectToDestroy(string args)
    {
        listOfObjectsToDestroy.Add(args);
    }


    #region SAVE/LOAD FUNCTION


    public static void SaveScene()
    {
        SceneInfo sceneInfo = SceneInfo.CreateInstance<SceneInfo>();

        sceneInfo.SceneIndex = SceneManager.GetActiveScene().buildIndex;
        sceneInfo.SceneName = SceneManager.GetActiveScene().name;

        DataBaseManager.SaveScene(sceneInfo);
    }

    public static void LoadScene()
    {
        SceneInfo sceneInfo = DataBaseManager.LoadScene();

        SceneManager.LoadScene(sceneInfo.SceneName, LoadSceneMode.Single);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneInfo.SceneName));
    }


    public static void SaveObjectsToDestroy()
    {
        DataBaseManager.SaveObjectsToDestroy(listOfObjectsToDestroy);
    }

    public static List<string> LoadObjectsToDestroy()
    {
        listOfObjectsToDestroy.Clear();

        listOfObjectsToDestroy.AddRange(DataBaseManager.LoadObjectsToDestroy());

        return listOfObjectsToDestroy;
    }


    public static void SaveInventory(List<HDK_InventorySlot> listaOggettiInventario)
    {
        DataBaseManager.SaveAllItemsToDB(listaOggettiInventario);
    }

    public static List<ItemAttribute> LoadInventory()
    {
        return DataBaseManager.LoadAllItemsFromDB();
    }


    public static void SavePlayerInfo(PlayerInfoDB playerInfo)
    {
        DataBaseManager.SavePlayerInfo(playerInfo);
    }

    public static PlayerInfoDB LoadPlayerInfo()
    {
        return DataBaseManager.LoadPlayerInfo();
    }

    #endregion


}
