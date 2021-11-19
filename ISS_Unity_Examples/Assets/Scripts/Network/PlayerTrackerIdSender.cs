using Mirror;

public class PlayerTrackerIdSender : NetworkBehaviour 
{
    [Client]
    private void Start() 
    {
        if(!isLocalPlayer) return;

        var trackerId = CommandLineParser.trackerSerial;
        Send(netId, trackerId);
    }

    [Command]
    private void Send(uint playerNetworkId, string trackerId)
    {
        PlayerManager.Instance.BindPlayerToTracker(playerNetworkId, trackerId);
    }
}
