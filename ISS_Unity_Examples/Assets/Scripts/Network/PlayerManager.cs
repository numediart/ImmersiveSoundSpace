using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : MonoBehaviour 
{
    private Dictionary<uint, Player> _players = new Dictionary<uint, Player>();
    private static PlayerManager _instance;
    public static PlayerManager Instance => _instance;

    [Server]
    private void Awake() 
    {
        if(_instance) return;

        _instance = this;
    }

    [Server]
    public void BindPlayerToTracker(uint playerNetworkId, string trackerId)
    {
        if(_players.ContainsKey(playerNetworkId)) return;

        var newPlayer = new Player{
            trackerSerial = trackerId,
            tracker = TrackersManagerOSC.Instance.GetTracker(trackerId),
            playerTransform = GetPlayer(playerNetworkId).transform
        };

        _players.Add(playerNetworkId, newPlayer);
        Debug.Log($"Added player <color=cyan>{playerNetworkId} - {trackerId}</color>");
    }

    [Server]
    public void Disconnect(uint playerNetworkId)
    {
        if(!_players.ContainsKey(playerNetworkId))
        {
            Debug.LogWarning($"Tried to remove player <color=cyan>{playerNetworkId}</color> but this player was not found");
            return;
        }

        _players.Remove(playerNetworkId);
    }

    [Server]
    private GameObject GetPlayer(uint id)
    {
        return NetworkServer.spawned[id].gameObject;
    }

    [Server]
    private void Update() 
    {
        foreach (KeyValuePair<uint, Player> player in _players)
        {
            Transform playerTransform = player.Value.playerTransform;
            TrackerObject tracker = player.Value.tracker;
            if (tracker != null)
            {
                playerTransform.position = tracker.updatedPos;
                playerTransform.rotation = tracker.updatedQuat;
            }
        }
    }


    [Server]
    public void SetPlayerTrackerObject(string trackerSerial, TrackerObject tracker)
    {
        uint id = GetPlayerNetIdFromTrackerSerial(trackerSerial);
        if(id > 0)
        {
            _players[id].tracker = tracker;
        }
    }


    private uint GetPlayerNetIdFromTrackerSerial(string trackerSerial)
    {
        foreach (KeyValuePair<uint, Player> player in _players)
        {
            if (player.Value.trackerSerial == trackerSerial)
                return player.Key;
        }
        return 0;
    }


    private class Player
    {
        public string trackerSerial;
        public TrackerObject tracker;
        public Transform playerTransform;
    }
}