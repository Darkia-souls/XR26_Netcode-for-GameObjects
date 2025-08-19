using UnityEngine;
using Unity.Netcode;

public class PlayerChat : NetworkBehaviour
{
    [SerializeField] private Player player;
	private bool chatSubscribed = false;
    
    public override void OnNetworkSpawn()
    {            
            base.OnNetworkSpawn();

            //Get reference to the Player on the same object
			player = GetComponent<Player>();
			if (player == null)
			{
            Debug.LogError("No Player component found on PlayerChat GameObject.");
            return;
        	}			

			if (IsOwner && !chatSubscribed)
			{
				if (ChatUI.Instance != null)
				{
					ChatUI.Instance.OnMessageSubmit += SendChatMessage;
					chatSubscribed = true;
				}
			}

    }   

	private void Update()
	{
		if (!chatSubscribed && ChatUI.Instance != null)
		{
			ChatUI.Instance.OnMessageSubmit += SendChatMessage;
			chatSubscribed = true;
			Debug.Log("Subscribed PlayerChat to ChatuUI");
		}
	}
        
        
    public void SendChatMessage(string text)
    {   
        if (string.IsNullOrWhiteSpace(text)) return;
		if (!IsOwner) return;
       SendMessageServerRpc(text);     
    }
       
    [ServerRpc(RequireOwnership = false)]
    public void SendMessageServerRpc(string text, ServerRpcParams rpcParams = default)
        
    {
        if (string.IsNullOrWhiteSpace(player.playerName.Value.ToString()))
        {
            Debug.LogWarning("Player name not set. Assigning fallback name.");
           player.playerName.Value = "Player" + OwnerClientId;
        }
            ReceiveMessageClientRpc(player.playerName.Value.ToString(), text);
    }
    
    [ClientRpc]
    public void ReceiveMessageClientRpc(string name, string text)
    {
        if (ChatUI.Instance != null)
        {
            ChatUI.Instance.CreateChatMessage(name, text);    
        }
         
    }
        
}
