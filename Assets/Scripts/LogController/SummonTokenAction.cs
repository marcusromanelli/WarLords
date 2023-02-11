using System.Text;
using UnityEngine;

public struct SummonTokenAction : IGameAction
{
	public Player ownerPlayer;
	public CardObject currentToken;
	public Vector2 position;
	public uint manaCost;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append(ownerPlayer.GetName());
		str.Append(" summoned ");
		str.Append(currentToken.GetName());
		str.Append(" on position ");
		str.Append(position);
		str.Append(" by spending ");
		str.Append(manaCost);
		str.Append(" of mana.");

		return str.ToString();
	}
	public static SummonTokenAction Create(CardObject currentToken, Vector2 position, uint manaCost)
	{
		return new SummonTokenAction()
		{
			ownerPlayer = currentToken.Player,
			currentToken = currentToken,
			position = position,
			manaCost = manaCost,
		};
	}
}