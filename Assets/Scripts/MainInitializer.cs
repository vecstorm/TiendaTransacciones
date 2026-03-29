using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInitializer : MonoBehaviour
{

    void Start()
    {
		int characterID = PlayerPrefs.GetInt("SelectedCharacterID", -1);

		if (characterID == -1)
		{
			Debug.LogError("No hay personaje seleccionado.");
			return;
		}

		InventoryDBManager.Instance.Initialice(characterID);

		CoinsManagerUI.Instance.RefreshCoins();
	}


}
