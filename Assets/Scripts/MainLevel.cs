using System;
using Unity.Netcode;
using UnityEngine;
using Unity.Mathematics;
using Random = System.Random;

public class MainLevel : NetworkBehaviour {
    private Random random;
    private PlayerManager playerManager;
    private GameObject[] spawnPoints;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Camera areaCamera;
    
    private void Start() {
        random = new Random();
        areaCamera.enabled = false;
        areaCamera.GetComponent<AudioListener>().enabled = !IsClient;
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (IsServer) {
            SpawnPlayers();
        }
    }

    private void SpawnPlayers() {
        foreach (NetworkPlayerInfo player in playerManager.AllPlayers) { 
            SpawnPlayer(player.ClientId);
        }
    }

    private void SpawnPlayer(ulong clientId) {
        GameObject playerSpawn = Instantiate(playerPrefab, GetRandomPosition(), Quaternion.identity);
        playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    private Vector3 GetRandomPosition() {
        int index = random.Next(0, spawnPoints.Length - 1);
        return spawnPoints[index].transform.position;
    }
}