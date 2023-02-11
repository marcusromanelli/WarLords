using System.Text;

public struct GraveyardHabilityAction : IGameAction
{
	public Player currentPlayer;
	public int numberDiscard;
	public int numberDrawn;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append(currentPlayer.name);
		str.Append(" discarted ");
		str.Append(numberDiscard);
		str.Append(" and drawn ");
		str.Append(numberDrawn);

		return str.ToString();
	}
	public static GraveyardHabilityAction Create(Player currentPlayer, int numberDiscarded, int numberDrawn)
	{
		return new GraveyardHabilityAction()
		{
			currentPlayer = currentPlayer,
		};
	}
}