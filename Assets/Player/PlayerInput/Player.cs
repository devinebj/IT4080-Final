using System;
using ParrelSync.NonCore;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using NetworkObject = Unity.Netcode.NetworkObject;

public class Player : NetworkBehaviour {
    public event Action<float> OnPlayerHealthChanged;
    public NetworkVariable<float> playerHealth = new();
    [SerializeField] float movementSpeed = 6;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private Rigidbody rigidBody;
    private PlayerInputActions playerInput;
    private float xRotation;

    private void Awake() {
        playerInput = new PlayerInputActions();
        playerInput.Player.Shoot.performed += Shoot;
    }

    public void Start() {
        NetworkInit();
        playerHealth.Value = 100;
        OnPlayerHealthChanged?.Invoke(playerHealth.Value);
    }

    private void OnEnable() {
        playerInput.Enable();
    }

    private void OnDisable() {
        playerInput.Disable();
    }

    private void Update() {
        Vector2 moveInput = playerInput.Player.WASD.ReadValue<Vector2>();
        Vector2 mouseInput = playerInput.Player.Mouse.ReadValue<Vector2>();

        if (IsOwner) {
            Move(moveInput);
            Rotate(mouseInput);
        }
    }

    private void NetworkInit() {
        playerCamera.gameObject.SetActive(IsOwner);
        Cursor.lockState = IsOwner ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = IsOwner;
    }

    private void Move(Vector2 movementInput) {
        Vector3 movement = new(movementInput.x, 0, movementInput.y);
        transform.Translate(movementSpeed * Time.deltaTime * movement);
    }

    private void Rotate(Vector2 rotationInput) {
        float mouseX = rotationInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = rotationInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    private void Shoot(InputAction.CallbackContext obj) {
        if (!IsOwner) return;
        bulletSpawner.FireServerRpc();
    }

    private void OnCollisionEnter(Collision collision) {
        if (IsServer) {
            ServerHandleCollision(collision);
        }
    }

    private void ServerHandleCollision(Collision collision) {
        ulong ownerId = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId;
        Player other = NetworkManager.Singleton.ConnectedClients[ownerId].PlayerObject.GetComponent<Player>();
        collision.gameObject.GetComponent<NetworkObject>().Despawn();

        TakeDamage(10);
    }
    
    private void TakeDamage(float damage) {
        if (CheckForDeath(playerHealth.Value, damage)) {
            NetworkObject.Despawn();
        } else {
            playerHealth.Value -= damage;
        }
    }

    private bool CheckForDeath(float health, float damage) {
        return health - damage <= 0;
    }
}