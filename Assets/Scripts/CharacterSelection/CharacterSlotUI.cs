using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSlotUI : MonoBehaviour
{
	public TMP_Text nameText;
	public GameObject deleteButton;
	public GameObject selectButton;
	public GameObject createButton; 
	private int characterID; 
	private CharacterSelectedUI manager;


	public void Setup(int id, string name, CharacterSelectedUI mgr) 
	{ 
		characterID = id; 
		manager = mgr; 
		nameText.text = name; 
		deleteButton.SetActive(true); 
		selectButton.SetActive(true);
		createButton.SetActive(false); 
	}

	public void SetupCreate(CharacterSelectedUI mgr) 
	{ 
		manager = mgr;
		nameText.text = "Crear personaje";
		deleteButton.SetActive(false);
		selectButton.SetActive(false);
		createButton.SetActive(true);
	}

	public void OnSelect() 
	{ 
		manager.SelectCharacter(characterID); 
	}

	public void OnDelete() 
	{
		manager.DeleteCharacter(characterID); 
	}

	public void OnCreate() 
	{
		manager.OpenCreatePanel(); 
	}
}
