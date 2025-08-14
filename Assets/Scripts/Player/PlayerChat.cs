using UnityEngine;
using Unity.Netcode;

public class PlayerChat : NetworkBehaviour
{
    [SerializeField] private Player player;
    
    public override void OnNetworkSpawn()
    {            
            base.OnNetworkSpawn();
            if(!IsOwner) return;
            
            ChatUI.Instance.OnMessageSubmit += SendChatMessage;
    }   
        
        
    public void SendChatMessage(string text)
    {            
       SendMessageServerRpc (text);     
    }
       
    [ServerRpc]
    public void SendMessageServerRpc(string text)
        
    {            
            ReceiveMessageClientRpc(player.playerName.Value.ToString(), text);
    }
    
    [ClientRpc]
    
    public void ReceiveMessageClientRpc(string name, string text)
    {            
           ChatUI.Instance.CreateChatMessage(name, text);     
    }
        
}
