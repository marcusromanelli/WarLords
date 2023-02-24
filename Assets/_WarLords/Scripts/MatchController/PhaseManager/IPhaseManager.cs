using System;

public interface IPhaseManager 
{
    public void Setup(Player localPlayer, Player remotePlayer, Battlefield Battlefield);
    public bool CanPlayerInteract(Player player);
}
