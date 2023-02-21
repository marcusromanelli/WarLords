using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScreen : MonoBehaviour
{
    public void GoToCardVisualizer()
    {
        SceneController.LoadLevel(MenuScreens.CardVisualizer);
    }
    public void GoToDeckManager()
    {
        SceneController.LoadLevel(MenuScreens.DeckManager);
    }
    public void GoToStartGame()
    {
        SceneController.LoadLevel(MenuScreens.StartGame);
    }
}
