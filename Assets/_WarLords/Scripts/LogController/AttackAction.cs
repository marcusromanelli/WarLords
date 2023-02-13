using System.Text;

public struct AttackAction : IGameAction
{
	public uint damage;
	public Player currentPlayer;
	public CardObject attackerCard;
	public IAttackable target;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		var cardName = attackerCard.Data.Name;

		str.Append(currentPlayer.name);
		str.Append("'s " + cardName + " attacked ");
		str.Append(target.GetName());
		str.Append(" with ");
		str.Append(damage);
		str.Append(" damage");

		return str.ToString();
	}
	public static AttackAction Create(uint damage, CardObject attacker, IAttackable target)
	{
		return new AttackAction()
		{
			damage = damage,
			currentPlayer = attacker.Player,
			attackerCard = attacker,
			target = target
		};
	}
}