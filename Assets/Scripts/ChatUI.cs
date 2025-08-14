using UnityEngine;
using TMPro;

public class ChatUI : MonoBehaviour
{
    public static ChatUI Instance {get; private set;}

    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private GameObject chatMsgPrefab;
    [SerializeField] private Transform content;
    
    public UnityAction<string> OnMessageSubmit;
    
    void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else 
            { 
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            
            chatInput.onSubmit.AddListener(OnMessageSubmit);
        }
    
    public void CreateChatMessage(string name, string msg)
     {
         var chatMsgObject = Instantiate(chatMsgPrefab, content, false);
         chatMsgObject.GetComponent<TMP_Text>().text = 
         $"[{name}] : {msg}";
     }   
        
    
}
