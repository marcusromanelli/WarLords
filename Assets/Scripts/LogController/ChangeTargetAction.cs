using System.Text;

public struct ChangeTargetAction : IGameAction
{
	public Player currentPlayer;
	public CardObject currentToken;
	public IAttackable newTarget;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append(currentPlayer.GetName());
		str.Append("'s ");
		str.Append(currentToken.GetName());

		if (newTarget == null)
		{
			str.Append("'s target has been removed.");
		}
		else
		{
			str.Append("'s target switched to");
			str.Append(newTarget);
		}

		return str.ToString();
	}
	public static ChangeTargetAction Create(CardObject currentToken, IAttackable target)
	{
		return new ChangeTargetAction()
		{
			currentPlayer = currentToken.Player,
			currentToken = currentToken,
			newTarget = target
		};
	}
}