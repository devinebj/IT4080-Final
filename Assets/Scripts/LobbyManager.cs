using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour {
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    public static LobbyManager Instance;

    private void Start() {
        Instance = this;
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }
    
    public void StartHost() => NetworkManager.Singleton.StartHost();
    public void StartClient() => NetworkManager.Singleton.StartClient();

    public LobbyManager SubscribeToLobbyEvents() {
        Lobby.Instance.OnStartGameButtonClicked += OnGameStarted;
        return this;
    }
    
    private void OnServerStarted() {
        NetworkManager.SceneManager.LoadScene("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void OnGameStarted() {
        Debug.Log("Game Started!");
        NetworkManager.SceneManager.LoadScene("MainLevel", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    
    private void OnClientStarted() {
        if (!IsHost) {
            clientButton.GetComponentInChildren<TextMeshProUGUI>().text = "Waiting for host";
        }
    }
}