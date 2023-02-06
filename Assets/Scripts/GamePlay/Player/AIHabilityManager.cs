using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class AIHabilityManager : HabilityManager
{
	public void UseManaHability(CardObject cardObject)
    {
        OnReleasedOnManaPool(cardObject);
    }
}
