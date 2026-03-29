using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class InventoryDBManager : MonoBehaviour
{
    public static InventoryDBManager Instance;

    public List<InventoryItem> inventory = new List<InventoryItem>();
    private Conection db;

    private string dbPath;
    private int currentCharacterID;
    private int currentInventoryID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            db = FindObjectOfType<Conection>();
        }
        else      
            Destroy(gameObject);
    }

    public void Initialice(int userID)
    {
		currentCharacterID = userID;

		dbPath = "URI=file:" + Application.persistentDataPath + "/DataBaseInventori.db";

		currentInventoryID = LoadInventoryIDFromDB(userID);
		if (GetCoins() == 0)
			AddCoins(1000);

		LoadInventoryItems();
		InventoryManagerUI.Instance.RefreshUI();
	}

    /// <summary>
    /// Con este metodo añadiremos y actualizaremos los items del inventario y guardaremos los datos en la DB
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="amount"></param>
    public void AddItem(int itemID, int amount = 1)
    {
        //buscamos si el item existe en la base de datos de los scriptableobjects
        ItemSO item = ItemDatabase.instance.GetItemByID(itemID);

        if (item == null)
        {
            Debug.LogWarning("Item no encontrado: " + itemID);
            return;
        }

        int remaining = amount;

        foreach (var invItem in inventory)
        {
            if (invItem.item.id != itemID) continue;
            if (invItem.quantity >= item.MaxStack) continue;

            int space = item.MaxStack - invItem.quantity;
            int toAdd = Mathf.Min(space, remaining);

            invItem.quantity += toAdd;
            UpdateItemInDB(itemID, invItem.quantity);
            remaining -= toAdd;

            if (remaining <= 0)
            {
                InventoryManagerUI.Instance.RefreshUI();
                return;
            }
        }

        while (remaining > 0)
        {
            int toAdd = Mathf.Min(remaining, item.MaxStack);

            inventory.Add(new InventoryItem(item, toAdd));
            InsertItemDB(itemID, toAdd);

            remaining -= toAdd;
        }
        //actualizamos la UI
        InventoryManagerUI.Instance.RefreshUI();
    }

    /// <summary>
    /// Metodo con el que reduciremos la cantidad o eliminaremos el item
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="amount"></param>
    public void RemoveItemFull(int itemID)
    {
        InventoryItem existingItem = inventory.Find(i => i.item.id == itemID);

        if (existingItem == null)
            return;

        inventory.Remove(existingItem);
        DeleteItemFronDB(itemID);

        InventoryManagerUI.Instance.RefreshUI();
    }

    public void RemoveItem(int itemID, int amount = 1)
    {
        InventoryItem existingItem = inventory.Find(i => i.item.id == itemID);

        if (existingItem == null)
            return;

        existingItem.quantity -= amount;
        if (existingItem.quantity <= 0)
        {
            inventory.Remove(existingItem);
            DeleteItemFronDB(itemID);
        }
        else
        {
            UpdateItemInDB(itemID, existingItem.quantity);
        }

        InventoryManagerUI.Instance.RefreshUI();
    }

    public void LoadInventoryID()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT InventoryID FROM Inventories WHERE CharacterID  = @charID";
                cmd.Parameters.Add(new SqliteParameter("@charID", currentCharacterID));

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    currentInventoryID = reader.GetInt32(0);
                    reader.Close();
                }
                else
                {
                    reader.Close();
                    cmd.CommandText = "INSERT INTO Inventories (CharacterID) VALUES (@charID)";
                    cmd.ExecuteNonQuery();
                    //nos va a devolver el ultimo id de autoincremento
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    currentInventoryID = System.Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            conn.Close();
        }
    }

    public void LoadInventoryItems()
    {
        inventory.Clear();

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT ItemID, Quantity FROM InventoryItems WHERE InventoryID = @invID";
                cmd.Parameters.Add(new SqliteParameter("@invID", currentInventoryID));

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int itemID = reader.GetInt32(0);
                    int quantity = reader.GetInt32(1);

                    ItemSO item = ItemDatabase.instance.GetItemByID(itemID);

                    if (item != null)
                    {
                        inventory.Add(new InventoryItem(item, quantity));
                    }
                }
            }
        }
    }

    private void InsertItemDB(int itemID, int quantity)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO InventoryItems (InventoryID, ItemID, Quantity) VALUES (@invID, @itemID, @qty)";
                cmd.Parameters.Add(new SqliteParameter("@invID", currentInventoryID));
                cmd.Parameters.Add(new SqliteParameter("@itemID", itemID));
                cmd.Parameters.Add(new SqliteParameter("@qty", quantity));
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }

    private void UpdateItemInDB(int itemID, int quantity)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE InventoryItems SET Quantity = @qty WHERE InventoryID = @invID AND ItemID = @itemID";
                cmd.Parameters.Add(new SqliteParameter("@qty", quantity));
                cmd.Parameters.Add(new SqliteParameter("@invID", currentInventoryID));
                cmd.Parameters.Add(new SqliteParameter("@itemID", itemID));
                cmd.ExecuteNonQuery();

            }
            conn.Close();
        }
    }

    private void DeleteItemFronDB(int itemID)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM InventoryItems WHERE InventoryID = @inv AND ItemID = @item";
                cmd.Parameters.Add(new SqliteParameter("@inv", currentInventoryID));
                cmd.Parameters.Add(new SqliteParameter("@item", itemID));
                cmd.ExecuteNonQuery();
            }
        }
    }

    public int GetCoins()
    {
        int coins = 0;

        db.ConectionDB();
        using (var cmd = db.CreateCommand())
        {
            cmd.CommandText = "SELECT Coins FROM Character WHERE Id = @id";
            cmd.Parameters.Add(new SqliteParameter("@id", currentCharacterID));

            var result = cmd.ExecuteScalar();
            coins = result != null ? int.Parse(result.ToString()) : 0;
        }
        db.DisconectDB();

        return coins;
    }

    public void AddCoins(int amount)
    {
        db.ConectionDB();
        using (var cmd = db.CreateCommand())
        {
            cmd.CommandText = "UPDATE Character SET Coins = Coins + @amount WHERE Id = @id";
            cmd.Parameters.Add(new SqliteParameter("@amount", amount));
            cmd.Parameters.Add(new SqliteParameter("@id", currentCharacterID));
            cmd.ExecuteNonQuery();
        }
        db.DisconectDB();
    }

    public bool SpendCoins(int amount)
    {
        int current = GetCoins();
        if (current < amount) return false;

        db.ConectionDB();
        using (var cmd = db.CreateCommand())
        {
            cmd.CommandText = "UPDATE Character SET Coins = Coins - @amount WHERE Id = @id";
            cmd.Parameters.Add(new SqliteParameter("@amount", amount));
            cmd.Parameters.Add(new SqliteParameter("@id", currentCharacterID));
            cmd.ExecuteNonQuery();
        }
        db.DisconectDB();

        return true;
    }

    public bool TryBuyItem(int itemID)
    {
        ItemSO item = ItemDatabase.instance.GetItemByID(itemID);
        if (item == null) return false;

        int price = item.price;

        db.ConectionDB();
        db.BeginTransaction();

        try
        {

            using (var cmd = db.CreateTransactionCommand())
            {
                cmd.CommandText = "UPDATE Character SET Coins = Coins - @price WHERE Id = @id";
                cmd.Parameters.Add(new SqliteParameter("@price", price));
                cmd.Parameters.Add(new SqliteParameter("@id", currentCharacterID));
                cmd.ExecuteNonQuery();
            }

            AddItemTransaction(itemID, 1);

            db.Commit();
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error en transacción de compra: " + ex.Message);
			db.Rollback();
            return false;
        }
        finally
        {
			db.DisconectDB();

		}
	}

    private void AddItemTransaction(int itemID, int amount)
    {
		ItemSO item = ItemDatabase.instance.GetItemByID(itemID);
		if (item == null) return;

		int remaining = amount;

		// 1. Intentar acumular en pilas existentes
		foreach (var invItem in inventory)
		{
			if (invItem.item.id != itemID) continue;
			if (invItem.quantity >= item.MaxStack) continue;

			int space = item.MaxStack - invItem.quantity;
			int toAdd = Mathf.Min(space, remaining);

			invItem.quantity += toAdd;

			// Actualizar en DB usando la transacción activa
			using (var cmd = db.CreateTransactionCommand())
			{
				cmd.CommandText = "UPDATE InventoryItems SET Quantity = @qty WHERE InventoryID = @inv AND ItemID = @item";
				cmd.Parameters.Add(new SqliteParameter("@qty", invItem.quantity));
				cmd.Parameters.Add(new SqliteParameter("@inv", currentInventoryID));
				cmd.Parameters.Add(new SqliteParameter("@item", itemID));
				cmd.ExecuteNonQuery();
			}

			remaining -= toAdd;
			if (remaining <= 0)
				return;
		}

		// 2. Crear nuevas pilas si aún queda cantidad por añadir
		while (remaining > 0)
		{
			int toAdd = Mathf.Min(remaining, item.MaxStack);

			// Añadir a memoria
			inventory.Add(new InventoryItem(item, toAdd));

			// Insertar en DB usando la transacción activa
			using (var cmd = db.CreateTransactionCommand())
			{
				cmd.CommandText = "INSERT INTO InventoryItems (InventoryID, ItemID, Quantity) VALUES (@inv, @item, @qty)";
				cmd.Parameters.Add(new SqliteParameter("@inv", currentInventoryID));
				cmd.Parameters.Add(new SqliteParameter("@item", itemID));
				cmd.Parameters.Add(new SqliteParameter("@qty", toAdd));
				cmd.ExecuteNonQuery();
			}

			remaining -= toAdd;
		}
	}

	public bool SellItem_Transaction(int itemID, int quantity, int sellPrice)
	{
		db.ConectionDB();
		db.BeginTransaction();

		try
		{
			using (var cmd = db.CreateTransactionCommand())
			{
				cmd.CommandText = "UPDATE InventoryItems SET Quantity = Quantity - @qty WHERE InventoryID = @inv AND ItemID = @item";
				cmd.Parameters.Add(new SqliteParameter("@qty", quantity));
				cmd.Parameters.Add(new SqliteParameter("@inv", currentInventoryID));
				cmd.Parameters.Add(new SqliteParameter("@item", itemID));
				cmd.ExecuteNonQuery();
			}

			using (var cmd = db.CreateTransactionCommand())
			{
				cmd.CommandText = "DELETE FROM InventoryItems WHERE InventoryID = @inv AND ItemID = @item AND Quantity <= 0";
				cmd.Parameters.Add(new SqliteParameter("@inv", currentInventoryID));
				cmd.Parameters.Add(new SqliteParameter("@item", itemID));
				cmd.ExecuteNonQuery();
			}

			using (var cmd = db.CreateTransactionCommand())
			{
				cmd.CommandText = "UPDATE Character SET Coins = Coins + @price WHERE Id = @id";
				cmd.Parameters.Add(new SqliteParameter("@price", sellPrice));
				cmd.Parameters.Add(new SqliteParameter("@id", currentCharacterID));
				cmd.ExecuteNonQuery();
			}

			db.Commit();
			return true;
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Error en venta: " + ex.Message);
			db.Rollback();
			return false;
        }
        finally
        {
			db.DisconectDB();

		}
	}

	private int LoadInventoryIDFromDB(int characterID)
	{
		int invID = -1;

		db.ConectionDB();

		// 1. Intentar obtener el inventario existente
		using (var cmd = db.CreateCommand())
		{
			cmd.CommandText = "SELECT InventoryID FROM Inventories WHERE CharacterID = @charID";
			cmd.Parameters.Add(new SqliteParameter("@charID", characterID));

			var result = cmd.ExecuteScalar();
			if (result != null)
			{
				invID = int.Parse(result.ToString());
			}

		}

		// 2. Si no existe, crearlo
		if (invID == -1)
		{
			using (var cmd = db.CreateCommand())
			{
				cmd.CommandText = @"
                INSERT INTO Inventories (CharacterID)
                VALUES (@charID);
                SELECT last_insert_rowid();";

				cmd.Parameters.Add(new SqliteParameter("@charID", characterID));

				invID = int.Parse(cmd.ExecuteScalar().ToString());
			}
		}
		db.DisconectDB();
		return invID;
	}


}
