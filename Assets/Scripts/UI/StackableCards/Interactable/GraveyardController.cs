using UnityEngine;

public class GraveyardController : InteractiveDeck
{
    private void Awake()
    {
        base.Awake();
    }
    public override void Update()
    {
        base.Update();
    }
    protected override void OnReleaseCard(CardObject cardObject)
    {
        //if (!isMouseOver)
        //    return;

        //if (!Player.hasCondition(ConditionType.DiscartCard))
        //{
        //    GameConfiguration.PlaySFX(GameConfiguration.denyAction);
        //    Debug.LogWarning("You cannot discart a cart without a reason.");
        //    return;
        //}


        //Player.DiscartCardFromHand(cardObject.GetCardData());
    }
}
