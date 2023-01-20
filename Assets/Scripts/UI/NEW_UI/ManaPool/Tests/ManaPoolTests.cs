using NUnit.Framework;

public class ManaPoolTests
{
	[Test]
	public void ManaPool_IncreaseMaxMana()
	{
		ManaPool manaPool = new ManaPool();

		manaPool.IncreaseMaxMana();
		manaPool.IncreaseMaxMana();

		Assert.IsTrue(manaPool.MaxMana == 2);

		Assert.IsTrue(manaPool.CurrentMana == 2);
	}

	[Test]
	public void ManaPool_Spend1Mana()
	{
		ManaPool manaPool = new ManaPool();

		manaPool.IncreaseMaxMana();
		manaPool.IncreaseMaxMana();

		manaPool.SpendMana(1);

		Assert.IsTrue(manaPool.MaxMana == 2);
		
		Assert.IsTrue(manaPool.CurrentMana == 1);
	}

	[Test]
	public void ManaPool_Spend2Mana()
	{
		ManaPool manaPool = new ManaPool();

		manaPool.IncreaseMaxMana();
		manaPool.IncreaseMaxMana();

		manaPool.SpendMana(2);

		Assert.IsTrue(manaPool.MaxMana == 2);

		Assert.IsTrue(manaPool.CurrentMana == 0);
	}


	[Test]
	public void ManaPool_RestopeSpentMana()
	{
		ManaPool manaPool = new ManaPool();

		manaPool.IncreaseMaxMana();
		manaPool.IncreaseMaxMana();

		manaPool.SpendMana(2);

		manaPool.RestoreSpentMana();

		Assert.IsTrue(manaPool.MaxMana == 2);

		Assert.IsTrue(manaPool.CurrentMana == 2);
	}
}
