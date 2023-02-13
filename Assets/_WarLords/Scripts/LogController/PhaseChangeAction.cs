using System.Text;

public struct PhaseChangeAction : IGameAction
{
	public Phase currentPhase;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append("Changed phase. Next phase is: ");
		str.Append(currentPhase.GetPhaseType().ToString());

		return str.ToString();
	}
	public static PhaseChangeAction Create(Phase currentPhase)
	{
		return new PhaseChangeAction()
		{
			currentPhase = currentPhase,
		};
	}
}