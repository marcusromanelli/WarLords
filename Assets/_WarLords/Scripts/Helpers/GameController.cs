public class GameController : Singleton<GameController>
{
    public static UserDeck LocalPlayerDeck { get; private set; }
    public static UserDeck RemotePlayerDeck { get; private set; }

    public static void SetPlayersDeck(UserDeck localPlayer, UserDeck remotePlayer)
    {
        LocalPlayerDeck = localPlayer;
        RemotePlayerDeck = remotePlayer;
    }
}
