using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour {
    public NetworkList<NetworkPlayerInfo> AllPlayers;
    public static PlayerManager Instance;

    private void Awake() {
        Instance = this;
        AllPlayers = new NetworkList<NetworkPlayerInfo>();
    }

    private void Start() {
        DontDestroyOnLoad(gameObject);
        if (IsServer) {
            StartServer();
            NetworkManager.Singleton.OnClientConnectedCallback += ServerOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += ServerOnClientDisconnected;
        }
    }
    
    private void StartServer() {
        NetworkPlayerInfo host = new(NetworkManager.LocalClientId);
        host.Ready = true;
        host.PlayerName = "The Host";
        AllPlayers.Add(host);
    }
    
    private void ServerOnClientConnected(ulong clientId) {
        NetworkPlayerInfo client = new(clientId);
        client.Ready = false;
        client.PlayerName = $"Player {clientId}";
        AllPlayers.Add(client);
    }
    
    private void ServerOnClientDisconnected(ulong clientId) {
        var index = FindPlayerIndex(clientId);
        if (index != -1) { AllPlayers.RemoveAt(index); }
    }
    
    public int FindPlayerIndex(ulong clientId) {
        var index = 0;
        var found = false;

        while (index < AllPlayers.Count && !found) {
            if (AllPlayers[index].ClientId == clientId) {
                found = true;
            } else {
                index++;
            }
        }
        
        if (!found) { index--; }
        return index;
    }
    
    public void UpdateReady(ulong clientId, bool ready) {
        int index = FindPlayerIndex(clientId);
        if (index == -1) { return; }

        NetworkPlayerInfo info = AllPlayers[index];
        info.Ready = ready;
        AllPlayers[index] = info;
    }
    
    public void UpdatePlayerName(ulong clientId, string playerName) {
        int index = FindPlayerIndex(clientId);
        if (index == -1) { return; }

        NetworkPlayerInfo info = AllPlayers[index];
        info.PlayerName = playerName;
        AllPlayers[index] = info;
    }
    
    public NetworkPlayerInfo GetInfo(ulong clientId) {
        NetworkPlayerInfo toReturn = new(ulong.MaxValue);
        int index = FindPlayerIndex(clientId);
      
        if (index != -1) { toReturn = AllPlayers[index]; }
        return toReturn;
    }
    
    public bool AllPlayersReady() {
        bool theyAre = true;
        int index = 0;

        while (theyAre && index < AllPlayers.Count) {
            theyAre = AllPlayers[index].Ready;
            index++;
        }

        return theyAre;
    }
}
