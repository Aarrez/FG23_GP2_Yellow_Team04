using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customization : MonoBehaviour
{
    public void PlayerCustomization()
    {
        GameManager.instance.UpdateGameState(GameManager.gameState.playerCustomizationState);
    }

    public void KayakCustomization()
    {
        GameManager.instance.UpdateGameState(GameManager.gameState.kayakCustomizationState);
    }

    public void BackToCustom()
    {
        GameManager.instance.UpdateGameState(GameManager.gameState.customizationState);
    }
}
