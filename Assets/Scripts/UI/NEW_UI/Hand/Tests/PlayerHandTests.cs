using NUnit.Framework;

public class PlayerHandTests
{
    Card GetRandomCard()
    {
        var card = new Card();

        card.CardID = UnityEngine.Random.Range(0, 9999);

        return card;
    }
    [Test]
    public void PlayerHandTests_AddCard_CheckCount()
    {
        Card card = GetRandomCard();

        PlayerHand hand = new PlayerHand();

        hand.AddCard(card);

        Assert.AreEqual(1, hand.Count);
    }
    [Test]
    public void PlayerHandTests_Add2Cards_CheckCount()
    {
        Card card1 = GetRandomCard();
        Card card2 = GetRandomCard();

        PlayerHand hand = new PlayerHand();

        hand.AddCard(card1);
        hand.AddCard(card2);

        Assert.AreEqual(2, hand.Count);
    }
    [Test]
    public void PlayerHandTests_Add_And_DiscardCard()
    {
        Card card = GetRandomCard();

        PlayerHand hand = new PlayerHand();

        hand.AddCard(card);

        Card discarded = hand.DiscardCard();

        Assert.AreEqual(card, discarded);
        Assert.AreEqual(0, hand.Count);
    }
    [Test]
    public void PlayerHandTests__Add_And_Discard2Cards()
    {
        Card card1 = GetRandomCard();
        Card card2 = GetRandomCard();

        PlayerHand hand = new PlayerHand();

        hand.AddCard(card1);
        hand.AddCard(card2);

        Card[] discarded = hand.DiscardCards(2);

        Assert.AreEqual(card2, discarded[0]);
        Assert.AreEqual(card1, discarded[1]);

        Assert.AreEqual(0, hand.Count);
    }
    [Test]
    public void PlayerHandTests_Add_And_GetCards()
    {
        Card card1 = GetRandomCard();
        Card card2 = GetRandomCard();

        PlayerHand hand = new PlayerHand();

        hand.AddCard(card1);
        hand.AddCard(card2);

        Card[] cards = hand.GetCards();

        Assert.AreEqual(card2, cards[0]);
        Assert.AreEqual(card1, cards[1]);
    }

}
