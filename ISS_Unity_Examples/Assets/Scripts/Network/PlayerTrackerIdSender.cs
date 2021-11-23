using Mirror;

public class PlayerTrackerIdSender : NetworkBehaviour
{
    [ClientCallback]
    private void Start()
    {
        if (!isLocalPlayer) return;

        var trackerId = CommandLineParser.trackerSerial;
        Send(netId, trackerId);
    }

    [Command]
    private void Send(uint playerNetworkId, string trackerId)
    {
        PlayerManager.Instance.BindPlayerToTracker(playerNetworkId, trackerId);
    }

    [ServerCallback]
    private void OnDestroy()
    {
        PlayerManager.Instance.Disconnect(netId);
    }
}
