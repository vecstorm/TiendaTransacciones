using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DBInitializer : MonoBehaviour
{
    private void Awake()
    {
        string dbName = "DataBaseInventori.db";
        string persistentPath = Path.Combine(Application.persistentDataPath, dbName);

        if (File.Exists(persistentPath)) return; //si existe el archivo no hacemos nada

        //cargamos la db desde resources
        TextAsset dbAsset = Resources.Load<TextAsset>("Database/DataBaseInventori");

        if (dbAsset != null)
        {
            File.WriteAllBytes(persistentPath, dbAsset.bytes);
        }
        else
        {
            Debug.LogError("no se encontro la base de datos");
        }
    }
}
