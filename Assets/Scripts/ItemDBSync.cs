using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDBSync : MonoBehaviour
{
    private string dbPath;

    private void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/DataBaseInventori.db";
        SyncItems();
    }

    /// <summary>
    /// Metodo que se encarga de crear y añadir los Items en la base de datos, si ya existen les asigna su Id a cada SO 
    /// </summary>
    private void SyncItems()
    {
        //recorremos todos los SciptableObjects y comprueba si estan en la base de datos
        // si no estan los añade y obtiene el ID
        ItemSO[] items = Resources.LoadAll<ItemSO>("Items");

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();

            foreach (var item in items)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM Items WHERE Name = @name";
                    cmd.Parameters.Add(new SqliteParameter("@name", item.itemName));

                    var result = cmd.ExecuteScalar();

                    if(result == null)
                    {
                        //insertamos un item nuevo
                        cmd.Parameters.Clear();
                        cmd.CommandText = "INSERT INTO Items (Name, Description) VALUES (@name, @desc)";
                        cmd.Parameters.Add(new SqliteParameter("@name", item.itemName));
                        cmd.Parameters.Add(new SqliteParameter("@desc", item.description));
                        cmd.ExecuteNonQuery();

                        //obtenemos el ID generado
                        cmd.Parameters.Clear();
                        cmd.CommandText = "SELECT last_insert_rowid()";
                        item.id = System.Convert.ToInt32(cmd.ExecuteScalar());

                        Debug.Log("Item registrado: " + item.itemName + " con ID " + item.id);
                    }
                    else
                    {
                        //si ya existe le asigan una ID al ScriptableObject
                        item.id = System.Convert.ToInt32(result);
                        Debug.Log("Item ya existía: " + item.itemName + " (ID: " + item.id + ")");
                    }
                }
            }
            conn.Close();
        }
    }
}
