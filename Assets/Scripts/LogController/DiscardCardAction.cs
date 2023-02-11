using System.Text;

public struct DiscardCardAction : IGameAction
{
	public Player currentPlayer;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append(currentPlayer.name);
		str.Append(" discarted 1 card");

		return str.ToString();
	}
	public static DiscardCardAction Create(Player currentPlayer)
	{
		return new DiscardCardAction()
		{
			currentPlayer = currentPlayer,
		};
	}
}