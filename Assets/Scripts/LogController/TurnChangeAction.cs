using System.Text;

public struct TurnChangeAction : IGameAction
{
	public Player currentPlayer;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append("Changed turn. Next player is: ");
		str.Append(currentPlayer.name);

		return str.ToString();
	}
	public static TurnChangeAction Create(Player currentPlayer)
	{
		return new TurnChangeAction()
		{
			currentPlayer = currentPlayer,
		};
	}
}