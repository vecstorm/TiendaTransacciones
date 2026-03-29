using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    GameObject logOutButton;

    public void LogOut()
    {
        SceneManager.LoadScene("Login");

	}
}
