using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public float moveSpeed = 5f;

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

    [ServerRpc]
    private void MoveServerRpc(Vector3 move, ServerRpcParams rpcParams = default)
    {
        // Apply movement on the server
        transform.position += move;
    }
}
