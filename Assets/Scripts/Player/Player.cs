using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Player : NetworkBehaviour
{

    [SerializeField] private PlayerChat playerChat;

	public NetworkVariable<float> moveSpeed = 
		new NetworkVariable<float>(
			value: 5f,
			NetworkVariableReadPermission.Everyone,
			NetworkVariableWritePermission.Owner
		);
   

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
        
        Debug.Log($"This player name is {playerName.Value}");

		//Subscribe to move speed changes
		moveSpeed.OnValueChanged += OnMoveSpeedChanged;

		//Request server to change speed
		ChangeMoveSpeedServerRPC(20f);
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


        Vector3 move = input * moveSpeed.Value * Time.deltaTime;

        // Send the movement to the server
        MoveServerRpc(move);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 move, ServerRpcParams rpcParams = default)
    {
        // Apply movement on the server
        transform.position += move;
    }
    
	[ServerRpc]
    private void ChangeMoveSpeedServerRPC( float newMoveSpeed)
    {
        moveSpeed.Value = newMoveSpeed;
    }
    
    public void OnMoveSpeedChanged(float oldValue, float newValue)
    {
        Debug.Log("$The movespeedvalue has changed from {oldValue} to {newValue} ");
    }
}
