using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simon : MonoBehaviour
{
    //grounds
    [SerializeField] private GameObject rZone; //0
    [SerializeField] private GameObject gZone; //1
    [SerializeField] private GameObject yZone; //2
    [SerializeField] private GameObject bZone; //3

    [SerializeField] private GameObject[] platforms;

    private int initTime = 10;
    private int timer = 0;
    private int timerLoops = -1;
    private bool cooldown = false;
    private bool eventStarted = false;

    private void Update()
    {
        if (!cooldown && !eventStarted)
        {
            StartCoroutine("BeginCommand");
        }
    }

    private IEnumerator BeginCommand()
    {
        eventStarted = true;
        timer = initTime;
        int commandNumber = 0; // Will add a random number once everything else gets implemented.
        Debug.Log(commandNumber);

        switch (commandNumber)
        {
            case 0: //colored zones
                StartCoroutine(ColoredZones(timer));
                Debug.Log("event is colored zones.");
                break;
            case 1: //platforms
                break;
            case 2: //bombs
                break;
            case 3: //do nothing
                break;
        }
   
        yield return null;
    }

    private IEnumerator ColoredZones(int time)
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
        Debug.Log("Stand on the " + zoneName);
        yield return new WaitForSeconds(time);
        Debug.Log("Time's Up!");

        foreach (GameObject zone in zones)
        {
            if (zone != selectedZone)
            {
                zone.SetActive(false);
            }
        }
        yield return new WaitForSeconds(3);
        Debug.Log("cooldown begins");

        foreach (GameObject zone in zones)
        {
            if (!zone.activeSelf)
            {
                zone.SetActive(true);
            }
        }
        StartCoroutine("Cooldown");
    }

    private IEnumerator TestEvent(int time)
    {
        // event setup
        Debug.Log("event in progress");
        yield return new WaitForSeconds(time);
        Debug.Log("time's up! event is over.");
        StartCoroutine("Cooldown");
    }

    private IEnumerator Cooldown()
    {
        eventStarted = false;
        cooldown = true;
        yield return new WaitForSeconds(5);
        Debug.Log("cooldown over");
        timerLoops++;
        if ((timerLoops % 2) == 0 && initTime >= 2)
        {
            initTime--;
            Debug.Log("Speed Up!");
        }
        cooldown = false;
    }

    /*    private IEnumerator BeginCommand()
        {
            int commandNumber = 0; // Will add a random number once everything else gets implemented.
            Debug.Log(commandNumber);

            switch (commandNumber)
            {
                case 0: //colored zones
                    StartCoroutine("ColoredZones");
                    break;
                case 1: //platforms
                    break;
                case 2: //bombs
                    break;
                case 3: //do nothing
                    break;
            }
            yield return null;
        }

        private IEnumerator ResetTimer()
        {
            timerLoops++;
            if (timerLoops > 0 && (timerLoops % 5) == 0) //for every 5 timer loops (but not at 0)
            {
                if (initTime !<= 4) //initTime can only go as low as 4 seconds. (after 30 loops)
                {
                    initTime -= 1f;
                }
            }
            timer = initTime;
            yield return null;
        }*/
}
