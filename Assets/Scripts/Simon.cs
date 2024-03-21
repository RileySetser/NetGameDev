using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Simon : MonoBehaviour
{
    //grounds
    [SerializeField] private GameObject rZone; //0
    [SerializeField] private GameObject gZone; //1
    [SerializeField] private GameObject yZone; //2
    [SerializeField] private GameObject bZone; //3

    [SerializeField] private GameObject[] platforms;

    [SerializeField] private TMP_Text UI;

    [SerializeField] private int timer = 10;
    private int timerLoops = -1;
    private bool cooldown = true;
    private bool eventStarted = false;
    private int cooldownTimer = 1;

    private void Start()
    {
        foreach (GameObject platform in platforms)
        {
            platform.SetActive(false);
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene_Singleplayer"))
        {
            UI.text = "Get ready!";
            StartCoroutine("StartGame");
            SP_Player player = (SP_Player)FindObjectOfType(typeof(SP_Player));
            if (player)
            {
                Debug.Log("player found");
            } else
            {
                Debug.Log("could not find player.");
            }
        }
    }

    private void Update()
    {
        if (!cooldown && !eventStarted)
        {
            StartCoroutine("BeginCommand");
        }
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);
        cooldown = !cooldown;
    }

    private IEnumerator BeginCommand()
    {
        eventStarted = true;
        int commandNumber = Random.Range(0, 3); // Will add a random number once everything else gets implemented.
        
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
                zoneName = "red zone.";
                break;
            case 1: //green
                selectedZone = gZone;
                zoneName = "green zone.";
                break;
            case 2: //yellow
                selectedZone = yZone;
                zoneName = "yellow zone.";
                break;
            case 3: //blue
                selectedZone = bZone;
                zoneName = "blue zone.";
                break;
        }
        UI.text = "Stand in the " + zoneName;
        yield return new WaitForSeconds(timer);
        UI.text = "TIME'S UP!!";

        foreach (GameObject zone in zones)
        {
            if (zone != selectedZone)
            {
                zone.SetActive(false);
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
                platform.transform.position = new Vector3(platform.transform.position.x, 5, platform.transform.position.z);
                highestPlatform = platform;
            }
            else
            {
                int rndHeight = Random.Range(1, 4);
                platform.transform.position = new Vector3(platform.transform.position.x, rndHeight, platform.transform.position.z);
            }
        }
        UI.text = "Stand on the highest platform!";
        // change heights of platforms
        // determine the highest platform
        // timer
        yield return new WaitForSeconds(timer);
        UI.text = "TIME'S UP!!";
        rZone.SetActive(false); gZone.SetActive(false); yZone.SetActive(false); bZone.SetActive(false);
        foreach (GameObject platform in platforms)
        {
            if (platform != highestPlatform)
            {
                platform.SetActive(false);
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
        yield return new WaitForSeconds(cooldownTimer);
        timerLoops++;
        if ((timerLoops % 2) == 0 && timer >= 2)
        {
            UI.text = "Speed Up!";
            timer--;
        }
        if ((timerLoops % 4) == 0 & cooldownTimer >= 1)
        {
            cooldownTimer--;
        }
        cooldown = false;
    }
}
