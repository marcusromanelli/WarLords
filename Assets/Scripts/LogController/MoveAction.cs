using System.Text;
using UnityEngine;

public struct MoveAction : IGameAction
{
	public Player currentPlayer;
	public CardObject currentToken;
	public Vector2 originalPosition;
	public Vector2 targetPosition;
	public float speed;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append(currentPlayer.GetName());
		str.Append("'s ");
		str.Append(currentToken.GetName());
		str.Append(" is moving from ");
		str.Append(originalPosition);
		str.Append(" to ");
		str.Append(targetPosition);
		str.Append(" at speed ");
		str.Append(speed);

		return str.ToString();
	}
	public static MoveAction Create(CardObject currentToken, Vector2 originalPosition, Vector2 targetPosition, float speed)
	{
		return new MoveAction()
		{
			currentPlayer = currentToken.Player,
			currentToken = currentToken,
			originalPosition = originalPosition,
			targetPosition = targetPosition,
			speed = speed
		};
	}
}