using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button readyBtn;

    private void Awake()
    {
        readyBtn.onClick.AddListener(() =>
        {
            PlayerReadyLogic.Instance.SetPlayerReady();
        });
    }
}
