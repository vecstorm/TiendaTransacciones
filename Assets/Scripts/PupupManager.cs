using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PupupManager : MonoBehaviour
{
    public GameObject errorPopup;
    public TextMeshProUGUI errorText;

    public GameObject messagePopup;
    public TextMeshProUGUI messageText;

    public GameObject registerPanel;

    public float popupDuration = 3f;

	public static PupupManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void ShowError(string message)
    {
        errorText.text = message;
        errorPopup.SetActive(true);
        StartCoroutine(HideAfterSeconds(errorPopup));
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
        StartCoroutine(HideAfterSeconds(messagePopup));
    }

    public void ShowRegisterPanel(GameObject panel)
    {
        panel.SetActive(true);    
    }

    public void ClosePanel()
    {
        registerPanel.SetActive(false);
    }


    private IEnumerator HideAfterSeconds(GameObject panel)
    {
        yield return new WaitForSeconds(popupDuration);
        panel.SetActive(false);
    }

}
