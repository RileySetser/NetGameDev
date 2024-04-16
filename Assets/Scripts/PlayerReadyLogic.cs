using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerReadyLogic : NetworkBehaviour
{
    public static PlayerReadyLogic Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDict;

    private void Awake()
    {
        Instance = this;
        playerReadyDict = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        PlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDict.ContainsKey(clientId) || !playerReadyDict[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
}
