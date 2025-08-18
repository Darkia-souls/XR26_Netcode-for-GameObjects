using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.Netcode.Components;
using Unity.Collections; 

public class Player : NetworkBehaviour
{

    [SerializeField] private PlayerChat playerChat;
	public bool isGrounded = true;
	public float jumpForce = 2.0f;
	
	public float groundDistanceCheck = 0.1f;

	public LayerMask groundMask;

	[SerializeField] private Rigidbody rigidBody;


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

	private void Awake()
	{
		groundMask = LayerMask.GetMask("Ground");
	}
    
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

		//If the space key is pressed and the player is grounded
		if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
		{
			Debug.Log("CLIENT: I want to jump!");
			JumpServerRpc();
		}

        Vector3 move = input * moveSpeed.Value * Time.deltaTime;

        // Send the movement to the server
        MoveServerRpc(move);
    	}

		//A server function that calls the jump function to preform jump logic
		[ServerRpc]
		private void JumpServerRpc()
		{
			Debug.Log("SERVER: The player wants to jump");
			//Actually Jump
			Jump();
		}
	
		//Apply velocity to the rigidbody to jump (works on either client or server)
		private void Jump()
		{
			var vel = rigidBody.linearVelocity;
			vel.y = 0f;
			vel.y += jumpForce; 
			//Because we have a *NetworkRigidbody* on the Player Prefab
			//The linearVeloctiy will be replicated (shared) to all connected clients
			rigidBody.linearVelocity = vel;
		}

	bool IsGrounded()
	{
		Vector3 origin = transform.position + Vector3.up * 0.05f;
		float radius = 0.25f;
		//Preform a SphereCast from the origin and the radius provided, only return results
		//that hits the ground layermask
		return Physics.SphereCast(origin, 
								radius, 
								Vector3.down, 
								out _, 
								10, 
								groundMask, 
								QueryTriggerInteraction.Ignore); 
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
