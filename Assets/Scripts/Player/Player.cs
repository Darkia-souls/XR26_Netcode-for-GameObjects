using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{

    [SerializeField] private PlayerChat playerChat;
    
    public float moveSpeed = 5f;

    public NetworkVariable<FixedString32Bytes> playerName =
        new NetworkVariable<FixedString32Bytes>(
            value: "",
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (PlayerSettings.PlayerName.Length <= 0)
        {
            Debug.Log("Cannot assign an empty name to player");
            return;
        }

        playerName.Value = PlayerSettings.PlayerName;
        
        Debug.Log($"This player name is {playerName.Value}")
    }

    private void Update()
    {
        // Only process input for the local player
        if (!IsOwner) return;

        Vector3 input = new Vector3(
            Input.GetAxis("Horizontal"),
            0f,
            Input.GetAxis("Vertical")
        );


        Vector3 move = input * moveSpeed * Time.deltaTime;

        // Send the movement to the server
        MoveServerRpc(move);
    }
    
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        moveSpeed.OnValueChanged += OnMoveSpeedChanged;
        ChangeMoveSpeedServerRPC(20);
        moveSpeedValue(20);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 move, ServerRpcParams rpcParams = default)
    {
        // Apply movement on the server
        transform.position += move;
    }
    
    private void ChangeMoveSpeedServerRPC( float newMoveSpeed)
    {
        newMoveSpeed.Value = newMoveSpeed;
    }
    
    public void OnMoveSpeedChanged(float oldValue, floar newValue)
    {
        Debug.Log("$The movespeedvalue has changed from {}");
    }
}
