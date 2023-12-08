using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletSpawner : NetworkBehaviour {
    public float ShotCountDown { get; private set; }
    public float roundsPerMinute = 120;
    [SerializeField] private Rigidbody bulletPrefab;
    [SerializeField] private float bulletSpeed = 500f;
    [SerializeField] private Transform camTransform;
    
    private void Update() {
        if (!IsServer) { return; }
        if (ShotCountDown <= 0) { return; }

        ShotCountDown -= Time.deltaTime;
    }
    
    [ServerRpc]
    public void FireServerRpc(ServerRpcParams rpcParams = default) {
        if (ShotCountDown > 0) { return; }

        Rigidbody newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        newBullet.velocity = camTransform.forward * bulletSpeed;
        newBullet.gameObject.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        Destroy(newBullet.gameObject, 3);

        ShotCountDown = 60/roundsPerMinute;
    }
}
