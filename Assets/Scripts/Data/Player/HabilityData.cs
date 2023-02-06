using UnityEngine;

public delegate HabilityManager GetHabilityManager();
public delegate Player GetPlayer();

public interface IHability
{
    public bool CanUse(GetHabilityManager getHabilitymanager, GetPlayer getPlayer);
    public void Use(GetHabilityManager getHabilitymanager, GetPlayer getPlayer);
}

public abstract class HabilityBase : ScriptableObject, IHability
{
    [SerializeField] protected HabilityTrigger trigger;
    [SerializeField] protected bool isUnique;

    public bool IsUnique => isUnique;
    public HabilityTrigger Trigger => trigger;

    public abstract bool CanUse(GetHabilityManager getHabilitymanager, GetPlayer getPlayer);
    public abstract void Use(GetHabilityManager getHabilitymanager, GetPlayer getPlayer);
}
