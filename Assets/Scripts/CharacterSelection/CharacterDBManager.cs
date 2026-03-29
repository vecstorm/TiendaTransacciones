using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class CharacterDBManager : MonoBehaviour
{
    public static CharacterDBManager Instance;
    private string dbPath;


    private void Awake() 
    { 
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
            dbPath = "URI=file:" + Application.persistentDataPath + "/DataBaseInventori.db"; 

        } else 
        { 
            Destroy(gameObject); 
        } 
    }

    public List<(int id, string name)> GetCharacters(int userID) 
    { 
        List<(int, string)> characters = new(); 

        using (var conn = new SqliteConnection(dbPath)) 
        { 
            conn.Open();

            using (var cmd = conn.CreateCommand())
            { 
                cmd.CommandText = "SELECT Id, Name FROM Character WHERE UserID = @u";
                cmd.Parameters.Add(new SqliteParameter("@u", userID));
                
                var reader = cmd.ExecuteReader();

                while (reader.Read()) 
                { 
                    characters.Add((reader.GetInt32(0), reader.GetString(1)));
                }

				reader.Close();
			}
		} 
        return characters; 
    }

    public void CreateCharacter(int userID, string name)
    { 
        using (var conn = new SqliteConnection(dbPath))
        { 
            conn.Open(); 
            
            using (var cmd = conn.CreateCommand()) 
            { 
                cmd.CommandText = "INSERT INTO Character (UserID, Name) VALUES (@u, @n)";
                cmd.Parameters.Add(new SqliteParameter("@u", userID));
                cmd.Parameters.Add(new SqliteParameter("@n", name));
                cmd.ExecuteNonQuery(); 
            }
        }
    }

    public void DeleteCharacter(int characterID)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open(); 
            
            using (var cmd = conn.CreateCommand())
            { 
                // Borrar inventario del personaje
                cmd.CommandText = "DELETE FROM InventoryItems WHERE InventoryID IN (SELECT InventoryID FROM Inventories WHERE CharacterID = @c)";
                cmd.Parameters.Add(new SqliteParameter("@c", characterID));

                cmd.ExecuteNonQuery(); 
                // Borrar inventario
                cmd.CommandText = "DELETE FROM Inventories WHERE CharacterID = @c";

                cmd.ExecuteNonQuery(); // Borrar personaje
                cmd.CommandText = "DELETE FROM Character WHERE Id = @c"; 

                cmd.ExecuteNonQuery();
            } 
        } 
    }      
    
}