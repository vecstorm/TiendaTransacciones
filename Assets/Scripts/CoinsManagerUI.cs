using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsManagerUI : MonoBehaviour
{
	public static CoinsManagerUI Instance;

	public TextMeshProUGUI coinsText;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		RefreshCoins();
	}

	public void RefreshCoins()
	{
		int coins = InventoryDBManager.Instance.GetCoins();
		coinsText.text = coins.ToString();
	}
}
