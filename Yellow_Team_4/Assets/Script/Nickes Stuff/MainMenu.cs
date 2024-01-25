using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   [SerializeField] private GameObject settingsMenu;

   [SerializeField] private GameObject mainMenu;
   //This script will mainly hold public voids
   //To use for the main menu buttons

   public void StartGame()
   {
      SceneManager.LoadScene(1);
   }

   public void Settings()
   {
      settingsMenu.SetActive(true);
      mainMenu.SetActive(false);
   }

   public void EnableMainMenu()
   {
      settingsMenu.SetActive(false);
      mainMenu.SetActive(true);
   }
}
