using UnityEngine;
using TMPro;
using UnityEngine.Events;


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
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            //Using onEndEdit to simulate "submit" when pressing Enter
            chatInput.onEndEdit.AddListener(HandleInputSubmit);
        }

    private void HandleInputSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;
        
        //Invoke event if subscribed
        OnMessageSubmit?.Invoke(text);
        
        //Clear input field
        chatInput.text = string.Empty;
    }
    
    public void CreateChatMessage(string name, string msg)
     {
         var chatMsgObject = Instantiate(chatMsgPrefab, content, false);
         TMP_Text textComponent = chatMsgObject.GetComponent<TMP_Text>();

         if (textComponent != null)
         {
             textComponent.text = $"[{name}]: {msg}";
         }
         else
         {
             Debug.LogWarning("Chat message prefab is missing a TMP_Text component.");
         }
     }   
        
    
}
