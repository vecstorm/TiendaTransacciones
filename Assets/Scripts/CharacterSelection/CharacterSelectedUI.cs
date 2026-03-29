using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CharacterSelectedUI : MonoBehaviour
{
	public GameObject characterSlotPrefab;
	public Transform characterContainer;
	public TMP_InputField newCharacterName;
	public GameObject createPanel;
	private int currentUserID;
	private List<(int id, string name)> characters;


	private void Start()
	{
		int userID = PlayerPrefs.GetInt("UserID", -1);

		Initialize(userID);
	}
	public void Initialize(int userID) 
	{ 
		currentUserID = userID; 
		RefreshUI(); 
	}

	public void RefreshUI()
	{
		//limpiamos los slots anteriores
		foreach (Transform child in characterContainer) 
			Destroy(child.gameObject);

		//cargamos los personajes del usuario
		characters = CharacterDBManager.Instance.GetCharacters(currentUserID);

		//instanciamos un slot por personaje
		foreach (var c in characters)
		{
			GameObject slot = Instantiate(characterSlotPrefab, characterContainer);
			slot.GetComponent<CharacterSlotUI>().Setup(c.id, c.name, this);
		}

		// Si hay menos de 3 personajes, mostrar botón de crear
		if (characters.Count < 3)
		{
			GameObject slot = Instantiate(characterSlotPrefab, characterContainer);
			slot.GetComponent<CharacterSlotUI>().SetupCreate(this);
		}
	}

	public void OpenCreatePanel() 
	{ 
		newCharacterName.text = "";
		createPanel.SetActive(true); 
	}

	public void CloseCreatePanel() 
	{ 
		createPanel.SetActive(false);
	}

	public void CreateCharacter()
	{
		if (string.IsNullOrWhiteSpace(newCharacterName.text)) return;
		CharacterDBManager.Instance.CreateCharacter(currentUserID, newCharacterName.text);

		CloseCreatePanel();
		RefreshUI();
	}

	public void DeleteCharacter(int characterID)
	{
		CharacterDBManager.Instance.DeleteCharacter(characterID);
		RefreshUI();
	}

	public void SelectCharacter(int characterID)
	{
		PlayerPrefs.SetInt("SelectedCharacterID", characterID); 
		SceneManager.LoadScene("Main");
	}
}