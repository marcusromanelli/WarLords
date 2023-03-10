using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameMenuScreen : MonoBehaviour
{
	[SerializeField] GameObject StartButton;
	[SerializeField] SelectDeckObject PlayerSelectionDeck;
	[SerializeField] SelectDeckObject EnemySelectionDeck;

	bool playerReady, enemyReady;
	UserDeck playerDeck, enemyDeck;
	public void Start()
	{
		var deckCollection = UserManager.GetData().GetDecks();
		var civs = CivilizationManager.GetAll();

		PlayerSelectionDeck?.Setup(deckCollection, civs, true, OnSelectedDeck);
		EnemySelectionDeck?.Setup(deckCollection, civs, false, OnSelectedDeck);
	}
	public void ReturnToMenu()
	{
		SceneController.LoadLevel(GameScreens.Menu);
	}
	public void StartGame()
    {
		GameController.SetPlayersDeck(playerDeck, enemyDeck);

		SceneController.LoadLevel(GameScreens.Stage_Swamp);
    }

	void OnSelectedDeck(bool localPlayer, UserDeck deckList)
    {
        if (localPlayer)
        {
			playerReady = true;
			playerDeck = deckList;
        }
        else
		{
			enemyReady = true;
			enemyDeck = deckList;
		}

		StartButton.SetActive(playerReady && enemyReady);
	}
}
