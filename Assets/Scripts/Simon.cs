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
    private SP_Player sp_player;

    [SerializeField] private int timer = 10;

    [SerializeField] private Transform mp_playerPrefab;
    private int score = 0;
    private int timerLoops = 0;
    private int cooldownTimer = 3;
    
    private string donot = "<color=#a30303>DON'T <color=#000000>";

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
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(mp_playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void Update()
    {
        if (!cooldown && !eventStarted && !gameOver)
        {
            StartCoroutine("BeginCommand");
        }
        if (isSingleplayer) scoreUI.text = "Score: " + score;
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);
        cooldown = !cooldown;
    }

    private IEnumerator DoOrDont() // Determines if "Simon Says" is added at the beginning.
    {
        int coinFlip = Random.Range(1, 100);
        donot = (coinFlip <= 50) ? "" : "<color=#a30303>DON'T <color=#000000>";
        donot_bool = (donot == "") ? false : true;
        yield return null;
    }

    private IEnumerator BeginCommand()
    {
        eventStarted = true;
        int commandNumber = Random.Range(0, 1); // Will add a random number once everything else gets implemented.
        StartCoroutine("DoOrDont");
        
        switch (commandNumber)
        {
            case 0: //colored zones
                StartCoroutine("ColoredZones");
                break;
            case 1: //platforms
                StartCoroutine("Platforms");
                break;
            case 2: //bombs
                StartCoroutine("Cooldown");
                break;
            case 3: //do nothing
                StartCoroutine("Cooldown");
                break;
            default:
                StartCoroutine("Cooldown");
                break;
        }
   
        yield return null;
    }

    private IEnumerator ColoredZones()
    {
        int selectedColor = Random.Range(0, 3);
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
        UI.text = donot + "Stand in the " + zoneName;
        yield return new WaitForSeconds(timer);
        UI.text = "TIME'S UP!!";

        foreach (GameObject zone in zones)
        {
            if (!donot_bool)
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
        UI.text = "";

        foreach (GameObject zone in zones)
        {
            if (!zone.activeSelf)
            {
                zone.SetActive(true);
            }
        }
        StartCoroutine("Cooldown");
    }

    private IEnumerator Platforms()
    {
        int platformSelected = Random.Range(0, 7);
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
        UI.text = donot + "Stand on the highest platform!";
        // change heights of platforms
        // determine the highest platform
        // timer
        yield return new WaitForSeconds(timer);
        UI.text = "TIME'S UP!!";
        rZone.SetActive(false); gZone.SetActive(false); yZone.SetActive(false); bZone.SetActive(false);
        foreach (GameObject platform in platforms)
        {
            if (!donot_bool)
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
        StartCoroutine("Cooldown");
    }

    private IEnumerator Cooldown()
    {
        eventStarted = false;
        cooldown = true;
        if (sp_player == null)
        {
            gameOver = true;
            UI.text = "<color=#FF6666>GAME OVER!";
        } else
        {
            UI.text = "...";
            score++;
            yield return new WaitForSeconds(cooldownTimer);
            timerLoops++;
            if ((timerLoops % 2) == 0 && timer >= 2)
            {
                UI.text = "<color=#FFBB66>Speed Up!";
                timer--;
            }
            if ((timerLoops % 4) == 0 & cooldownTimer >= 1)
            {
                cooldownTimer--;
            }
            cooldown = false;
        } 
    }
}
