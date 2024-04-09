using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_NetworkUI : NetworkBehaviour
{
    [SerializeField] private Button createBtn;
    [SerializeField] private Button joinBtn;
    [SerializeField] private Button SP_Btn;

    private void Awake()
    {
        createBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        });
        joinBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
        SP_Btn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GameScene_Singleplayer");
        });
    }
}
