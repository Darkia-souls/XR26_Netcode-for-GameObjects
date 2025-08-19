using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuDisplay : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string gameplaySceneName = "Gameplay";

    [SerializeField] private TMP_InputField nameInput;

    void Start()
    {
        nameInput.onValueChanged.AddListener(OnNameChanged);
    }

    private void OnNameChanged(string name)
    {
        PlayerSettings.PlayerName = name;
        Debug.Log($"Player name changed to {name}");
    }
    
    public void StartHost()
    {
        if (!string.IsNullOrWhiteSpace(nameInput.text))
            PlayerSettings.PlayerName = nameInput.text;
        else
        {
            PlayerSettings.PlayerName = "Host_" + Random.Range(0, 1000);
        }
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }

    public void StartClient()
    {
        if (!string.IsNullOrWhiteSpace(nameInput.text))
            PlayerSettings.PlayerName = nameInput.text;
        else
        {
            PlayerSettings.PlayerName = "Client_" + Random.Range(0, 1000);
        }
        NetworkManager.Singleton.StartClient();
    }
}
