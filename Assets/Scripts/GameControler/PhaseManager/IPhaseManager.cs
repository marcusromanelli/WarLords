using System;

public interface IPhaseManager 
{
    public void Setup(Player localPlayer, Player remotePlayer);
    public bool CanPlayerInteract(Player player);
}
