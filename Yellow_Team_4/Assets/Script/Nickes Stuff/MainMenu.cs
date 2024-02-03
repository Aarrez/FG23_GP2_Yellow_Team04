using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   //This script will mainly hold public voids
   //To use for the main menu buttons
   public void StartGame(int index)
   {
      SceneManager.LoadScene(index);
      GameManager.instance.UpdateGameState(GameManager.gameState.readyState);
   }

   public void LevelSelection()
   {
      GameManager.instance.UpdateGameState(GameManager.gameState.levelSelectionState);
   }

   public void Settings()
   {
      GameManager.instance.UpdateGameState(GameManager.gameState.settingsState);
   }

   public void Leaderboard()
   {
      GameManager.instance.UpdateGameState(GameManager.gameState.leaderboardState);
   }

   public void Customization()
   {
      GameManager.instance.UpdateGameState(GameManager.gameState.customizationState);
   }

   public void Store()
   {
      GameManager.instance.UpdateGameState(GameManager.gameState.storeState);
   }

   public void EnableMainMenu()
   {
      GameManager.instance.UpdateGameState(GameManager.gameState.mainmenuState);
   }

   public void SetSFXVolume(float volume)
   {
      AudioManager.instance.masterMixer.SetFloat("SFX", Mathf.Log10(volume)*20);
      PlayerPrefs.SetFloat("sfxVolume", volume);
   }
   public void SetMasterVolume(float volume)
   {
      AudioManager.instance.masterMixer.SetFloat("master", Mathf.Log10(volume)*20);
      PlayerPrefs.SetFloat("masterVolume", volume);
   }
   public void SetMusicVolume(float volume)
   {
      AudioManager.instance.masterMixer.SetFloat("music", Mathf.Log10(volume)*20);
      PlayerPrefs.SetFloat("musicVolume", volume);
   }

   private void Start()
   {
      SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume"));
      SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
      SetMasterVolume(PlayerPrefs.GetFloat("masterVolume"));
   }
}
