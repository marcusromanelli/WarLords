using System.Text;

public struct TokenDestroyedAction : IGameAction
{
	public Player ownerPlayer;
	public CardObject token;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append(ownerPlayer.GetName());
		str.Append("'s token");
		str.Append(token.GetName());
		str.Append(" has been destroyed.");

		return str.ToString();
	}
	public static TokenDestroyedAction Create(CardObject token)
	{
		return new TokenDestroyedAction()
		{
			ownerPlayer = token.Player,
			token = token,
		};
	}
}