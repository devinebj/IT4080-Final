using System;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerListItem : NetworkBehaviour {
    public ulong ClientId { get; private set; }
    public TextMeshProUGUI PlayerName;
    public GameObject KickButton;
    public TextMeshProUGUI ReadyText;
    private PlayerManager playerManager;
    private PlayerListItem listItem;
    public event Action<ulong> OnKickButtonClicked;

    public void SetUpPlayerListItem(GameObject newPlayerListItem, NetworkPlayerInfo info) {
        ClientId = info.ClientId;
        listItem = newPlayerListItem.GetComponent<PlayerListItem>();
        bool isHost = NetworkManager.LocalClientId == NetworkManager.ServerClientId;
        listItem.PlayerName.text = info.PlayerName.ToString();

        if (ClientId == NetworkManager.ServerClientId) {
            listItem.ReadyText.text = info.Ready ? "Ready" : "Not Ready";
            KickButton.SetActive(false);
        } else {
            listItem.ReadyText.text = info.Ready ? "Ready" : "Not Ready";
            KickButton.SetActive(isHost);
        }
    }

    public void UpdatePlayerUI(ulong clientId) {
        NetworkPlayerInfo info = PlayerManager.Instance.GetInfo(clientId);
        listItem.ReadyText.text = info.Ready ? "Ready" : "Not Ready";
        listItem.PlayerName.text = info.PlayerName.ToString();
    }

    public void OnKickClicked() {
        OnKickButtonClicked?.Invoke(ClientId);
    }
}
