using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class Conection : MonoBehaviour
{
	public static Conection Instance;
	private string dbPath;
    private IDbConnection connection;
	private IDbTransaction transaction;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}


	void Start()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "DataBaseInventori.db"); 
        if (!File.Exists(dbPath))
        {         
            File.Create(dbPath).Close();
            CreateDatabase(dbPath);
        }

    }

    public void CreateDatabase(string path)
    {
        using (var conn = new SqliteConnection("URI=file:" + dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Users (" +
                    "UserID INTEGER PRIMARY KEY AUTOINCREMENT, " + 
                    "UserName TEXT NOT NULL UNIQUE, " +
                    "Password TEXT NOT NULL);";
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Items (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Name TEXT NOT NULL UNIQUE, " +
                    "Description TEXT NOT NULL);";
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Character (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "UserID INTEGER NOT NULL, " +
                    "Name TEXT NOT NULL UNIQUE, " +
                    "Coins INTEGER NOT NULL DEFAULT 0 CHECK (Coins >= 0) , " +
                    "FOREIGN KEY(UserID) REFERENCES Users(UserID));";
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Inventories (" +
                    "InventoryID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "CharacterID INTEGER NOT NULL UNIQUE, " +
                    "FOREIGN KEY(CharacterID) REFERENCES Character(Id));";
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS InventoryItems (" +
                    "InventoryItemID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "InventoryID INTEGER NOT NULL, " +
                    "ItemID INTEGER NOT NULL, " +
                    "Quantity INTEGER NOT NULL DEFAULT 1, " +
                    "FOREIGN KEY(InventoryID) REFERENCES Inventories(InventoryID), " +
                    "FOREIGN KEY(ItemID) REFERENCES Items(Id));";
                cmd.ExecuteNonQuery();

            }
            conn.Close();
        }
        
    }

    public void ConectionDB()
    {
        connection = new SqliteConnection("URI=file:" + dbPath);
        connection.Open();
    }

    public void DisconectDB()
    {
        if (connection != null)
        {
            connection.Close();
        }
    }

    public IDbCommand CreateCommand()
    {
        return connection.CreateCommand();
    }

	public void BeginTransaction()
	{
		if (connection == null)
			ConectionDB();

		transaction = connection.BeginTransaction();
	}

	public void Commit()
	{
		transaction?.Commit();
		transaction = null;
	}

	public void Rollback()
	{
		transaction?.Rollback();
		transaction = null;
	}

	public IDbCommand CreateTransactionCommand()
	{
		var cmd = connection.CreateCommand();
		cmd.Transaction = transaction;
		return cmd;
	}


}
