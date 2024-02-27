using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_NetworkUI : NetworkBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button joinBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private TMP_Text networkModeText;

    
    private string text = "";

    private void Start()
    {
        serverBtn.onClick.AddListener(ServerClick);
        hostBtn.onClick.AddListener(HostClick);
        joinBtn.onClick.AddListener(JoinClick);
        startBtn.onClick.AddListener(StartClick);

        NetworkManager.OnServerStarted += OnServerStarted;
        NetworkManager.OnClientStarted += OnClientStarted;

        startBtn.gameObject.SetActive(false);
    }

    private void OnServerStarted()
    {
        startBtn.gameObject.SetActive(true);
    }

    private void OnClientStarted()
    {
        if (!IsHost)
        {
            startBtn.gameObject.SetActive(true);
        }
    }

    private void ServerClick()
    {
        NetworkManager.Singleton.StartServer();
    }

    private void HostClick()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void JoinClick()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void StartClick()
    {
        StartGame();
    }

    private void StartGame()
    {
        NetworkManager.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
