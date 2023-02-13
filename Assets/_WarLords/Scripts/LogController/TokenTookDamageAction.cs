using System.Text;

public struct TokenTookDamageAction : IGameAction
{
	public IAttackable element;
	public uint damage;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append(element.GetName());
		str.Append(" took ");
		str.Append(damage);
		str.Append(" damage.");


		return str.ToString();
	}
	public static TokenTookDamageAction Create(IAttackable element, uint damage)
	{
		return new TokenTookDamageAction()
		{
			element = element,
			damage = damage,
		};
	}
}