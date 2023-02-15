using UnityEngine;
using System.Collections;

public class GameConfiguration {
	public static int numberOfCardsToDrawEveryTurn = 1;
	public static int numberOfInitialDrawnCards = 8;
	public static int numberOfInitialMana = 3;
	public static int maxNumberOfCardsInHand = 8;
	public static int maxNumberOfCardsInManaPool = 12;
	public static uint startLife = 15;


	public static AudioClip drawCard;
	public static AudioClip buffCard;
	public static AudioClip energyToCard;
	public static AudioClip cardToEnergy;
	public static AudioClip confirmAction;
	public static AudioClip debuff;
	public static AudioClip denyAction;
	public static AudioClip explosion;
	public static AudioClip fireMagic;
	public static AudioClip heal;
	public static AudioClip movementBuff;
	public static AudioClip useEnergy;
	public static AudioClip Summon;
	public static AudioClip Walk;
	public static AudioClip Death;
	public static AudioClip Hit;

	public static void PlaySFX(AudioClip audio)
	{
		//GameConfiguration.Singleton.audioSource.PlayOneShot(audio);
	}
}