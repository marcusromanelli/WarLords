using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScreen : MonoBehaviour
{
    public void GoToCardVisualizer()
    {
        SceneController.LoadLevel(GameScreens.CardVisualizer);
    }
    public void GoToDeckManager()
    {
        SceneController.LoadLevel(GameScreens.DeckManager);
    }
    public void GoToStartGame()
    {
        SceneController.LoadLevel(GameScreens.StartGame);
    }
}
