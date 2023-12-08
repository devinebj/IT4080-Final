using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private Slider healthBar;
    
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        player.playerHealth.OnValueChanged += UpdatePlayerHealth;
    }

    private void UpdatePlayerHealth(float oldHealth, float newHealth) {
        healthBar.value = newHealth/100;
    }
}