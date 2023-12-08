using TMPro;
using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class Lobby : NetworkBehaviour {
    private GameObject playerListItem;
    public PlayerManager PlayerManager;
    public static Lobby Instance;
    public event Action OnStartGameButtonClicked;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject readyCheckBox;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private Transform playerList;
    [SerializeField] private TMP_InputField nameChangeTextBox;
    
    private void Start() {
        Instance = this;
        PlayerManager = PlayerManager.Instance;
        LobbyManager.Instance.SubscribeToLobbyEvents();
        
        if (IsServer) {
            ServerPopulateList();
            PlayerManager.AllPlayers.OnListChanged += ServerOnNetworkedPlayersChanged;
            startGameButton.SetActive(true);
            readyCheckBox.SetActive(false);

        } else {
            ClientPopulateList();
            PlayerManager.AllPlayers.OnListChanged += ClientOnNetworkedPlayersChanged;
            startGameButton.SetActive(false);
            readyCheckBox.SetActive(true);
        }
    }
    
    private void ServerOnNetworkedPlayersChanged(NetworkListEvent<NetworkPlayerInfo> changeEvent) {
        ServerPopulateList();
        startGameButton.GetComponent<Button>().enabled = PlayerManager.AllPlayersReady();
    }
    
    private void ClientOnNetworkedPlayersChanged(NetworkListEvent<NetworkPlayerInfo> changeEvent) {
        ClientPopulateList();
    }


    private void ServerPopulateList() {
        ClearList();

        foreach (NetworkPlayerInfo info in PlayerManager.AllPlayers) {
            GameObject newPlayerListItem = Instantiate(playerListItemPrefab, playerList);
            PlayerListItem listItem = newPlayerListItem.GetComponent<PlayerListItem>(); 
            listItem.SetUpPlayerListItem(newPlayerListItem, info);
            listItem.OnKickButtonClicked += ServerOnKickClicked;
        }
    }

    private void ClientPopulateList() {
        ClearList();

        foreach (NetworkPlayerInfo info in PlayerManager.AllPlayers) {
            GameObject newPlayerListItem = Instantiate(playerListItemPrefab, playerList);
            newPlayerListItem.GetComponent<PlayerListItem>().SetUpPlayerListItem(newPlayerListItem, info);
        }
    }

    private void ClearList() {
        foreach (Transform child in playerList) {
            Destroy(child.gameObject);
        }
    }

    public void ClickStartButton() {
        OnStartGameButtonClicked?.Invoke();
    }

    private void ServerOnKickClicked(ulong clientId) {
        NetworkManager.Singleton.DisconnectClient(clientId);
        ServerPopulateList();
    }

    public void UpdateReady(bool ready) {
        UpdateReadyServerRpc(ready);
    }

    public void UpdatePlayerName() {
        string newName = nameChangeTextBox.text;
        nameChangeTextBox.text = "";
        UpdatePlayerNameServerRpc(newName);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void UpdateReadyServerRpc(bool newValue, ServerRpcParams serverRpcParams = default) {
        PlayerManager.UpdateReady(serverRpcParams.Receive.SenderClientId, newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerNameServerRpc(string newValue, ServerRpcParams serverRpcParams = default) {
        PlayerManager.UpdatePlayerName(serverRpcParams.Receive.SenderClientId, newValue);
    }
}
