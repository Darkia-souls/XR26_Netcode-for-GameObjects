using UnityEngine;
using Unity.Netcode;

public class PlayerChat : NetworkBehaviour
{
    [SerializeField] private Player player;
    
    public override void OnNetworkSpawn()
    {            
            base.OnNetworkSpawn();
            if(!IsOwner) return;

            if (ChatUI.Instance != null)
            {
                ChatUI.Instance.OnMessageSubmit += SendChatMessage;
            }
            else
            {
                Debug.LogWarning("ChatUI instance not found. Chat messages will not be sent.");
            }
    }   
        
        
    public void SendChatMessage(string text)
    {   
        if (string.IsNullOrWhiteSpace(text)) return;
       SendMessageServerRpc (text);     
    }
       
    [ServerRpc]
    public void SendMessageServerRpc(string text)
        
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference not assigned in PlayerChat.");
            return;
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
