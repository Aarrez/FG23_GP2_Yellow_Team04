using GlobalStructs;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
   //This script will mainly hold public voids
   //To use for the main menu buttons
   private SoundVolume soundVolume;
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
      soundVolume.Sfx = volume;
      UserDataManager.SetSavedVolume.Invoke(soundVolume);
   }
   public void SetMasterVolume(float volume)
   {
      AudioManager.instance.masterMixer.SetFloat("master", Mathf.Log10(volume)*20);
      soundVolume.Master = volume;
      UserDataManager.SetSavedVolume.Invoke(soundVolume);
   }
   public void SetMusicVolume(float volume)
   {
      AudioManager.instance.masterMixer.SetFloat("music", Mathf.Log10(volume)*20);
      soundVolume.Music = volume;
      UserDataManager.SetSavedVolume.Invoke(soundVolume);
   }

   private void Start()
   {
      soundVolume = UserDataManager.GetSavedVolume.Invoke();
      SetSFXVolume(soundVolume.Sfx);
      SetMusicVolume(soundVolume.Music);
      SetMasterVolume(soundVolume.Master);
   }
}
