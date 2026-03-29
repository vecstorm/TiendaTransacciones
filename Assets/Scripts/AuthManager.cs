using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AuthManager : MonoBehaviour
{
    public Conection db;
    public PupupManager ui;

    [SerializeField] TMP_InputField userLogin;
    [SerializeField] TMP_InputField passwordLogin;
    [SerializeField] TMP_InputField userRegister;
    [SerializeField] TMP_InputField passwordRegister;

    public void Register()
    {
        if (passwordRegister.text.Length < 8)
        {
            ui.ShowError("La contraseña debe tener al menos 8 caracteres");
            return;
        }

        db.ConectionDB();
        var cmd = db.CreateCommand();
        cmd.CommandText = "INSERT INTO Users (Username, Password) VALUES (@u, @p)";
        cmd.Parameters.Add(new SqliteParameter("@u", userRegister.text));
        cmd.Parameters.Add(new SqliteParameter("@p", passwordRegister.text));

        try
        {
            cmd.ExecuteNonQuery();
            ui.ShowMessage("Usuario registrado correctamente");
        }
        catch
        {
            ui.ShowError("El usuario ya existe");
        }

        db.DisconectDB();
    }

    public void Login()
    {
        db.ConectionDB();
        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT UserID, Password FROM Users WHERE Username = @u";
        cmd.Parameters.Add(new SqliteParameter("@u", userLogin.text));

        var reader = cmd.ExecuteReader();

        if (!reader.Read())
        {
            ui.ShowError("Usuario no encontrado");
            db.DisconectDB();
            return;
        }

        int userID = reader.GetInt32(0);
        string storedPass = reader.GetString(1);

        if (storedPass != passwordLogin.text)
        {
            ui.ShowError("Contraseña incorrecta");
            db.DisconectDB();
            return;
        }

        db.DisconectDB();

		PlayerPrefs.SetInt("UserID", userID);
        SceneManager.LoadScene("CharacterSelect");

    }

    public void Logout()
    {
        SceneManager.LoadScene("Login");
    }

}
