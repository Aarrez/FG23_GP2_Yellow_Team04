
using System.Collections.Generic;
using System.Linq;
using GlobalStructs;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
   //This script will mainly hold public voids
   //To use for the main menu buttons
   private static SoundVolume soundVolume;
   public int totalstars = new int();
   public int totalcoins = new int();
   public TextMeshProUGUI starsTxt;
   public TextMeshProUGUI moneyTxt;

   public void Initialize()
   {
      GameManager.onGameStateChanged += DisplayStars;
   }

   public void StartGame(int index)
   {
      GameManager.instance.UpdateGameState(GameManager.gameState.loadingState);
      SceneManager.LoadSceneAsync(index);
      SceneManager.sceneLoaded += ReadyUp;
   }

   void ReadyUp(Scene scene, LoadSceneMode mode)
   {
      if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
      {
         GameManager.instance.UpdateGameState(GameManager.gameState.readyState);
      }
      else
      {
         GameManager.instance.UpdateGameState(GameManager.gameState.mainmenuState);
      }
   }

   void DisplayStars(GameManager.gameState state)
   {
      totalstars = 0;
      foreach (var level in  UserDataManager.upd.GetUserData().LevelData)
      {
         int temp = 0;
         for (int i = 0; i < level.Value.Count; i++)
         {
            if (level.Value[i].Starts == StarsEarned.Three)
            {
               temp = 3;
               break;
            }
            else if (level.Value[i].Starts == StarsEarned.Two)
            {
               temp = 2;
               continue;
            }
            else if (level.Value[i].Starts == StarsEarned.One)
            {
               if (temp < 2)
               {
                  temp = 1;  
               }
               continue;
            }
            else if (level.Value[i].Starts == StarsEarned.Zero)
            {
               continue;
            }
         }
         totalstars += temp;
      }
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

   public static void SetSFXVolume(float volume)
   {
      // AudioManager.instance.masterMixer.SetFloat("SFX", Mathf.Log10(volume)*20);
      FAudioMan.instance.SFXVolume = volume;      
      soundVolume.Sfx = volume;
      UserDataManager.SetSavedVolume.Invoke(soundVolume);
   }
   public static void SetMasterVolume(float volume)
   {
      // AudioManager.instance.masterMixer.SetFloat("master", Mathf.Log10(volume)*20);
      FAudioMan.instance.masterVolume = volume;
      soundVolume.Master = volume;
      UserDataManager.SetSavedVolume.Invoke(soundVolume);
   }
   public static void SetMusicVolume(float volume)
   {
      // AudioManager.instance.masterMixer.SetFloat("music", Mathf.Log10(volume)*20);
      FAudioMan.instance.musicVolume = volume;
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

   private void Update()
   {
      totalcoins = UserDataManager.upd.GetUserData().CurrentCurrency;
      starsTxt.text = totalstars.ToString();
      moneyTxt.text = totalcoins.ToString();
   }
}
