using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;

public class MainMenuManager : MonoBehaviour
{
    private void Update()
    {
        // Check for user input to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenu();
        }
    }

    public void MainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
}
