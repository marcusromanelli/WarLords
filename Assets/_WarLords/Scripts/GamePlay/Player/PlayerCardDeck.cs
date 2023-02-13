using UnityEngine;


[CreateAssetMenu(fileName = "PlayerCardDeck", menuName = "ScriptableObjects/Card/Player Deck", order = 1)]
public class PlayerCardDeck : ScriptableObject
{
    [SerializeField] Card[] cards;

    public Card[] Cards => cards;
}
