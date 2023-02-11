using System.Text;

public struct DrawCardAction : IGameAction
{
	public int number;
	public Player currentPlayer;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append(currentPlayer.name);
		str.Append(" drawed ");
		str.Append(number);
		str.Append(" card");
		str.Append((number > 1) ? "s" : "");

		return str.ToString();
	}
	public static DrawCardAction Create(Player currentPlayer, int number)
	{
		return new DrawCardAction()
		{
			number = number,
			currentPlayer = currentPlayer,
		};
	}
}