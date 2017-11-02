using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;


public class DataBaseManager:MonoBehaviour
{
    private string conn;

    private IDbConnection dbconn;
   
    public static string DATABASE_PATH = "/DATA/horrorgame.db";

    void Start()
    {       
        dbconn = null;
    }


    #region DataBase Function

    public void DBConnect()
    {
        conn = "URI=file:" + Application.dataPath + DATABASE_PATH;

        dbconn = (IDbConnection)new SqliteConnection(conn);

        dbconn.Open();

        Debug.Log("Connessione al DB effettuata.");
    }

    public void DBDisconnect()
    {
        dbconn.Close();
        dbconn = null;
        Debug.Log("Disconnesso dal DB.");
    }

    public void DeleteAllItems()
    {
        string sqlQueryInventory = "DELETE FROM INVENTORY;";
        try
        {
            DBConnect();
            Debug.Log("DeleteAllItems start");
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            dbcmd.CommandText = sqlQueryInventory;

            int deletedRow = dbcmd.ExecuteNonQuery();
            Debug.Log("Record rimossi: " + string.Format("{0}", deletedRow));

            Debug.Log("DeleteAllItems end");

            dbcmd.Dispose();
            dbcmd = null;

            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("Errore nell'esecuzione della query {0}\n - Eccezzione: {1}", sqlQueryInventory, ex)); 
            DBDisconnect();
        }
       
    }

    public void DeleteAllPlayerInfo()
    {
        string sqlQueryPlayerInfo = "DELETE FROM PLAYER_INFO;";
        try
        {
            DBConnect();
            Debug.Log("DeleteAllPlayerInfo start");
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            dbcmd.CommandText = sqlQueryPlayerInfo;

            int deletedRow = dbcmd.ExecuteNonQuery();
            Debug.Log("Record rimossi: " + string.Format("{0}", deletedRow));
            Debug.Log("DeleteAllPlayerInfo end");


            dbcmd.Dispose();
            dbcmd = null;

            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("Errore nell'esecuzione della query {0}\n - Eccezzione: {1}", sqlQueryPlayerInfo, ex)); 
            DBDisconnect();
        }
    }

    public void DeleteAllSceneInfo()
    {
        string sqlQuerySceneInfo = "DELETE FROM SCENE_INFO;";
        try
        {
            DBConnect();
            Debug.Log("DeleteAllSceneInfo start");
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            dbcmd.CommandText = sqlQuerySceneInfo;

            int deletedRow = dbcmd.ExecuteNonQuery();
            Debug.Log("Record rimossi: " + string.Format("{0}", deletedRow));
            Debug.Log("DeleteAllSceneInfo end");


            dbcmd.Dispose();
            dbcmd = null;

            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("Errore nell'esecuzione della query {0}\n - Eccezzione: {1}", sqlQuerySceneInfo, ex)); 
            DBDisconnect();
        }
    }

 
    public  void  SaveAllItemsToDB(List<HDK_InventorySlot> itemAttributeList)
    {
        try
        {
            //Per comodita'cancello tutto l'inventario
            DeleteAllItems();

            DBConnect();

            Debug.Log("SaveAllItemToDB start");

            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            foreach (HDK_InventorySlot itemAttribute  in itemAttributeList)
            {
                if (!itemAttribute.Empty)
                {
                    string itemSlot = itemAttribute.gameObject.name;
                    string itemName = itemAttribute.itemName;
                    string itemInfo = itemAttribute.itemInfo;
                    string type = itemAttribute.itemType.ToString();
  
                    string imagePath = itemAttribute.gameObject.GetComponent<Image>().mainTexture.name;
                    string itemIcon = imagePath;

                    int ammoQta = itemAttribute.AmmosQuantity;
                    string weaponTarget = (itemAttribute.WeaponTarget != null) ? itemAttribute.WeaponTarget.name : "";
                    float healthValue = itemAttribute.HealthValue;
                    string keyTarget = (itemAttribute.TargetObject != null) ? itemAttribute.TargetObject.name : "";


                    string values = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'", 
                                        itemSlot, itemName, itemInfo, type, itemIcon, ammoQta, weaponTarget, healthValue, keyTarget);

                    //Debug.Log("Valori: " + values); 

                    string sqlQuery = string.Format("INSERT INTO INVENTORY ('ITEM_SLOT','ITEM_NAME','ITEM_INFO','ITEM_TYPE','ITEM_ICON','AMMOS_QUANTITY','WEAPON_TARGET','HEALTH_VALUE','KEY_TARGET_OBJECT')" +
                                          " VALUES ({0});", values);

                    //Debug.Log("Query: " + sqlQuery);

                    dbcmd.CommandText = sqlQuery;

                    int insertedRow = dbcmd.ExecuteNonQuery();

                    if (insertedRow == 0)
                    {
                        Debug.Log("Errore nell'esecuzione della query!"); 
                    }
                    else
                    {
                        Debug.Log("Query eseguita!"); 
                    }

                }
            }

            dbcmd.Dispose();
            dbcmd = null;

            Debug.Log("SaveAllItemToDB end");
            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
            DBDisconnect();
        }           
    }


    public List<ItemAttribute> LoadAllItemsFromDB()
    {
        List<ItemAttribute> listAttribute = new List<ItemAttribute>();
      
        try
        {
            DBConnect();

            Debug.Log("LoadAllItemFromDB start");

            IDataReader reader = null;

            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            string sqlQuery = "SELECT * FROM INVENTORY ORDER BY ITEM_SLOT ASC;";

            dbcmd.CommandText = sqlQuery;

            reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                string slotname = reader.GetString(0);

                ItemAttribute slot = ItemAttribute.CreateInstance<ItemAttribute>();

                slot.SlotName = slotname;
                slot.ItemName = reader.GetString(1);
                slot.ItemInfo = reader.GetString(2);
                slot.ItemType = (ItemType)Enum.Parse(typeof(ItemType), reader.GetString(3));
               
                string imgName = reader.GetString(4);
                slot.ItemImageName = "IMG/Inventory/" + imgName;

                slot.AmmosQuantity = reader.GetInt32(5);

                slot.WeaponTargetName = reader.GetString(6);

                slot.HealthValue = reader.GetFloat(7);

                slot.TargetObjName = reader.GetString(8);

                listAttribute.Add(slot);
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;

            Debug.Log("LoadAllItemFromDB end");
            DBDisconnect();

           
        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
            DBDisconnect();
        }

        return listAttribute;   
    }


    public void SavePlayerInfo(PlayerInfoDB info)
    {
        try
        {
            //Per comodita'cancello tutte le info del player
            DeleteAllPlayerInfo();

            DBConnect();

            Debug.Log("SavePlayerInfo start");

            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            string values = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'", 
                                info.PositionInScene.x, info.PositionInScene.y, info.PositionInScene.z, 
                                info.RotationInScene.x, info.RotationInScene.y, info.RotationInScene.z, info.RotationInScene.w, info.CurrentHealth, info.TotalHealth);


            string sqlQuery = string.Format("INSERT INTO PLAYER_INFO ('POSITION_X','POSITION_Y','POSITION_Z','ROTATION_X','ROTATION_Y','ROTATION_Z','ROTATION_W','HEALTH_VALUE','HEALTH_TOTAL')" +
                                  " VALUES ({0});", values);


            dbcmd.CommandText = sqlQuery;

            int insertedRow = dbcmd.ExecuteNonQuery();

            if (insertedRow == 0)
            {
                Debug.Log("Errore nell'esecuzione della query!"); 
            }
            else
            {
                Debug.Log("Query eseguita!"); 
            }

            dbcmd.Dispose();
            dbcmd = null;

            Debug.Log("SavePlayerInfo end");
            DBDisconnect(); 
        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
            DBDisconnect();
        }
    }


    public PlayerInfoDB LoadPlayerInfo()
    {

        PlayerInfoDB info = PlayerInfoDB.CreateInstance<PlayerInfoDB>();

        try
        {
            DBConnect();

            Debug.Log("LoadPlayerInfo start");

            IDataReader reader = null;

            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            string sqlQuery = "SELECT * FROM PLAYER_INFO";

            dbcmd.CommandText = sqlQuery;

            reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                Vector3 pos = new Vector3(reader.GetFloat(0), reader.GetFloat(1), reader.GetFloat(2));

                info.PositionInScene = pos;

                Quaternion rotation = new Quaternion(reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6));

                info.RotationInScene = rotation;

                info.CurrentHealth = reader.GetFloat(7);
                info.TotalHealth = reader.GetFloat(8);          
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;

            Debug.Log("LoadPlayerInfo end");
            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
            DBDisconnect();
        }
            
        return info;
    }

    public void SaveScene(List<string> listOfObjectToDestroy)
    {
        try
        {
            DeleteAllSceneInfo();

            DBConnect();

            Debug.Log("SaveScene start");

            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            foreach (string itemName in listOfObjectToDestroy)
            {
                string values = string.Format("'{0}'", itemName);


                string sqlQuery = string.Format("INSERT INTO OBJECTS_TO_DESTROY ('NAME')" +
                                      " VALUES ({0});", values);


                dbcmd.CommandText = sqlQuery;

                int insertedRow = dbcmd.ExecuteNonQuery();

                if (insertedRow == 0)
                {
                    Debug.Log("Errore nell'esecuzione della query!"); 
                }
                else
                {
                    Debug.Log("Query eseguita!"); 
                }
            }


            dbcmd.Dispose();
            dbcmd = null;

            Debug.Log("SaveScene end");
            DBDisconnect(); 
        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
            DBDisconnect();
        }
    }

    public List<string> LoadScene()
    {
        List<string> listOfObjectToDestroy = new List<string>();

        try
        {
            DBConnect();

            Debug.Log("LoadScene start");

            IDataReader reader = null;

            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;

            string sqlQuery = "SELECT * FROM OBJECTS_TO_DESTROY";

            dbcmd.CommandText = sqlQuery;

            reader = dbcmd.ExecuteReader();

            string sceneName = "";
            int sceneIndex = 0;

            while (reader.Read())
            {
                sceneName = reader.GetString(0);
                listOfObjectToDestroy.Add(sceneName);
            }
                
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;

            Debug.Log("LoadScene end");
            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log("Errore nell'esecuzione della query!:\n" + ex); 
            DBDisconnect();
        }

        return listOfObjectToDestroy;
    }


    #endregion

}
