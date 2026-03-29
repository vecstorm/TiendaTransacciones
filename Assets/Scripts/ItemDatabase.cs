using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{

    public static ItemDatabase instance;

    public List<ItemSO> items = new List<ItemSO>();
    private Dictionary<int, ItemSO> itemDictionary = new Dictionary<int, ItemSO>();

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadItems();
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void LoadItems()
    {
        ItemSO[] loadedItems = Resources.LoadAll<ItemSO>("Items");

        items.Clear();
        itemDictionary.Clear();

        foreach (var item in loadedItems)
        {
            items.Add(item);
            itemDictionary[item.id] = item;
        }

        Debug.Log("numItems Cargados: " + items.Count);
    }

    public ItemSO GetItemByID(int id)
    {
        if (itemDictionary.TryGetValue(id, out ItemSO item))
            return item;

        Debug.Log("Id de item no encontrada: " + id);
        return null;
    }
}
