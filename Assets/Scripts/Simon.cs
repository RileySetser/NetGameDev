using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Simon : NetworkBehaviour
{
    //grounds
    [SerializeField] private GameObject rZone; //0
    [SerializeField] private GameObject gZone; //1
    [SerializeField] private GameObject yZone; //2
    [SerializeField] private GameObject bZone; //3

    [SerializeField] private GameObject[] platforms;

    [SerializeField] private TMP_Text UI;
    [SerializeField] private TMP_Text scoreUI = null;
    [SerializeField] private TMP_Text speedUI = null;
    private SP_Player sp_player;
    private Player[] players; //count players, if all players dead, game over

    [SerializeField] private int timer = 10;
    private NetworkVariable<int> networkTimer = new NetworkVariable<int>();

    [SerializeField] private Transform mp_playerPrefab;
    private int score = 0;
    private int timerLoops = 0;
    private int cooldownTimer = 3;

    private NetworkVariable<int> networkScore = new NetworkVariable<int>();

    private string donot = "";

    private bool isSingleplayer = false;
    private bool donot_bool = true;
    private bool cooldown = true;
    private bool eventStarted = false;
    private bool gameOver = false;

    private void Start()
    {
        foreach (GameObject platform in platforms)
        {
            platform.SetActive(false);
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene_Singleplayer"))
        {
            isSingleplayer = true;
            UI.text = "Get ready!";
            StartCoroutine("StartGame");
            sp_player = (SP_Player)FindObjectOfType(typeof(SP_Player));
        } else
        {
            // for multiplayer
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            networkScore.Value = 0;
            networkTimer.Value = 10;
        } else
        {
            networkScore.OnValueChanged += OnScoreChanged;
            networkTimer.OnValueChanged += OnTimerChanged;
        }
    }

    public void OnScoreChanged(int previous, int current)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");
        score = current;
        scoreUI.text = "Score: " + score;
    }

    public void OnTimerChanged(int previous, int current)
    {
        timer = current;
    }

    private void Update()
    {
        if (!IsServer)
        {
            if (!isSingleplayer)
            {
                return;
            } 
        }
        if (cooldown)
        {
            StartCoroutine("StartGame");
        }
        if (!cooldown && !eventStarted && !gameOver)
        {
            BeginCommand();
        }
        //scoreUI.text = "Score: " + score;
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);
        cooldown = !cooldown;
    }

    private string DontDisplay(bool dont_bool)
    {
        string dontText = dont_bool ? "<color=#a30303>DON'T <color=#000000>" : "";
        return dontText;
    }

    private bool DoOrDont()
    {
        int coinFlip = Random.Range(1, 100);
        bool dontChance = (coinFlip <= 50) ? false : true;
        return dontChance;
    }

    [ClientRpc]
    private void UpdateScoreClientRpc()
    {
        Debug.Log($"Network Score: {networkScore.Value} | Client Score: {score}");
        if (IsClient) { 
            scoreUI.text = "Score: " + networkScore.Value;
        }
        else
        {
            scoreUI.text = "Score: " + score;
        }
    }

    private void BeginCommand()
    {
        eventStarted = true;
        int commandNumber = Random.Range(0, 1); // Will add a random number once everything else gets implemented.
        
        bool dontChance = DoOrDont();
        string coroutineName = "";
        int coroutineNum = 0;
        
            switch (commandNumber)
            {
                case 0: //colored zones
                    coroutineNum = Random.Range(0, 3); //determines what zone is selected
                    coroutineName = "ColoredZones";
                    break;
                case 1: //platforms
                    coroutineNum = Random.Range(0, 7); //determines what platform is selected
                    coroutineName = "Platforms";
                    break;
                default:
                    coroutineName = "Cooldown";
                    break;
            }
        if (!isSingleplayer)
        {
            UpdateScoreClientRpc();
            StartCoroutineClientRpc(coroutineName, coroutineNum, dontChance);
        } else // if it is in singleplayer.
        {
            StartCoroutine(coroutineName, coroutineNum);
        }
    }

    [ClientRpc]
    private void StartCoroutineClientRpc(string name, int coroutineNum, bool dontChance)
    {
        
        object[] parms = new object[2]{ coroutineNum, dontChance };
        StartCoroutine(name, parms);
    }

    private IEnumerator ColoredZones(object[] parms)
    {
        int selectedColor = (int)parms[0];
        bool dontChance = (bool)parms[1];
        GameObject selectedZone = rZone;
        string zoneName = "";
        GameObject[] zones = { rZone, gZone, yZone, bZone };

        switch (selectedColor)
        {
            case 0: //red
                selectedZone = rZone;
                zoneName = "<color=#a30303>red zone.";
                break;
            case 1: //green
                selectedZone = gZone;
                zoneName = "<color=#0b7a01>green zone.";
                break;
            case 2: //yellow
                selectedZone = yZone;
                zoneName = "<color=#e6ae07>yellow zone.";
                break;
            case 3: //blue
                selectedZone = bZone;
                zoneName = "<color=#01157a>blue zone.";
                break;
        }
        string dontText = DontDisplay(dontChance);
        UI.text = dontText + "Stand in the " + zoneName;
        if (IsServer)
        {
            yield return new WaitForSeconds(networkTimer.Value);
        }
        else
        {
            yield return new WaitForSeconds(timer);
        }
        UI.text = "TIME'S UP!!";

        foreach (GameObject zone in zones)
        {
            if (!dontChance)
            {
                if (zone != selectedZone)
                {
                    zone.SetActive(false);
                }
            } else
            {
                selectedZone.SetActive(false);
            }
            
        }
        yield return new WaitForSeconds(3);

        foreach (GameObject zone in zones)
        {
            if (!zone.activeSelf)
            {
                zone.SetActive(true);
            }
        }
        cooldown = true;
        StartCoroutine("Cooldown");
    }

    private IEnumerator Platforms(object[] parms)
    {
        int platformSelected = (int)parms[0];
        bool dontChance = (bool)parms[1];
        GameObject highestPlatform = new GameObject();
        // activate platforms
        foreach (GameObject platform in platforms)
        {      
            platform.SetActive(true);


            if (platform == platforms[platformSelected])
            {
                platform.transform.position = new Vector3(platform.transform.position.x, 4, platform.transform.position.z);
                highestPlatform = platform;
            }
            else
            {
                float rndHeight = Random.Range(1, 3);
                platform.transform.position = new Vector3(platform.transform.position.x, rndHeight, platform.transform.position.z);
            }
        }
        string dontText = DontDisplay(dontChance);
        UI.text = dontText + "Stand on the highest platform!";
        // change heights of platforms
        // determine the highest platform
        // timer
        if (IsServer)
        {
            yield return new WaitForSeconds(networkTimer.Value);
        } else
        {
            yield return new WaitForSeconds(timer);
        }
        
        UI.text = "TIME'S UP!!";
        rZone.SetActive(false); gZone.SetActive(false); yZone.SetActive(false); bZone.SetActive(false);
        foreach (GameObject platform in platforms)
        {
            if (!dontChance)
            {
                if (platform != highestPlatform)
                {
                    platform.SetActive(false);
                }
            } else
            {
                highestPlatform.SetActive(false);
            }
            
        }
        yield return new WaitForSeconds(3);
        UI.text = "";
        rZone.SetActive(true); gZone.SetActive(true); yZone.SetActive(true); bZone.SetActive(true);
        foreach (GameObject platform in platforms)
        {
            if (platform.activeSelf)
            {
                platform.SetActive(false);
            }
        }
        cooldown = true;
        StartCoroutine("Cooldown");
    }

    private IEnumerator Cooldown()
    {
        eventStarted = false;
        if (sp_player == null && isSingleplayer)
        {
            gameOver = true;
            UI.text = "<color=#FF6666>GAME OVER!";
        } else
        {
            speedUI.text = "";
            if (IsServer)
            {
                networkScore.Value++;
            }
            timerLoops++;
            if ((timerLoops % 2) == 0 && timer >= 2)
            {
                speedUI.text = "<color=#FFBB66>Speed Up!";
                if (IsServer)
                {
                    networkTimer.Value--;
                }
            }
            if ((timerLoops % 4) == 0 && cooldownTimer >= 1)
            {
                cooldownTimer--;
            }
            yield return new WaitForSeconds(cooldownTimer);
            speedUI.text = "";
            cooldown = false;
        } 
    }

    
}
