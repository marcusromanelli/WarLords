using System.Text;

public struct ManaGenerateAction : IGameAction
{
	public Player currentPlayer;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append(currentPlayer.name);
		str.Append(" generated 1 mana");

		return str.ToString();
	}
	public static ManaGenerateAction Create(Player currentPlayer)
	{
		return new ManaGenerateAction()
		{
			currentPlayer = currentPlayer,
		};
	}
}