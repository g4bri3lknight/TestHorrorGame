using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEditor;


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
        string sqlQuery = "DELETE FROM INVENTORY;";

        try
        {
            DBConnect();
            Debug.Log("DeleteAllItems start");
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandType = CommandType.Text;
            dbcmd.CommandText = sqlQuery;
            int deletedRow = dbcmd.ExecuteNonQuery();
            Debug.Log("Record rimossi: " + string.Format("{0}", deletedRow));
            Debug.Log("DeleteAllItems end");
            DBDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("Errore nell'esecuzione della query {0}\n - Eccezzione: {1}", sqlQuery, ex)); 
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


    #endregion


}
