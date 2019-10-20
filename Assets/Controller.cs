using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using PunksUnlimited.DataModel;
using UnityEngine.UI;
using System.Linq;
public class Controller : MonoBehaviour
{
    // DPAD example received json
    //  {
    //    "element": "view-0-section-0-element-0",
    //      "data": {
    //      "pressed": false
    //      }
    //  }

    // BUILD example received json object
    //{
    //  "element": "view-0-section-1-element-0",
    //  "data": {
    //    "pressed": true,
    //    "Command": "BUILD"
    //  }
    //}

    public GameObject PlayerPrefab;
    public GameObject GameStartScreen;
    public GameObject VictoryScreen;

    public Text playerOneText;
    public Text playerTwoText;
    public Text playerThreeText;
    public Text playerFourText;

    public VictorTextController victorTextController;

    public SliderManager[] SliderManagers;
    public ResourceTextManager[] ResourceTextManagers;

    public MapController mapController;

    public EndGameUpdater endGameUpdate;

    int playerNumber = 0;

    Dictionary<int, CharacterData> CharacterControllers = new Dictionary<int, CharacterData>();

    bool GameRunning = false;
    public TimeUpdate timer;

    void Start()
    {
        
        timer.OnTick += Controller_OnTick;
        timer.Active = false;

        timer.CurrentTimeUpdated += Timer_CurrentTimeUpdated;

        endGameUpdate.SetMaxTime((int)timer.timeInterval);

        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += Instance_onConnect;
        AirConsole.instance.onDisconnect += Instance_onDisconnect;
    }

    private void Timer_CurrentTimeUpdated(object sender, float e)
    {
        endGameUpdate.UpdateTime(e);
    }

    private void Controller_OnTick(object sender, float e)
    {
        EndGame();
    }

    private void Instance_onDisconnect(int device_id)
    {

    }

    private void Instance_onConnect(int device_id)
    {
        var spawnPos = GetPlayerSpawnPos(playerNumber);
        mapController.Initilizeplayer(spawnPos);
        var playerClone = Instantiate(PlayerPrefab, new Vector3(spawnPos.x, 0, spawnPos.y), Quaternion.identity) as GameObject;
        var charControl = playerClone.GetComponent<CharacterData>();
        charControl.Initialize(spawnPos,device_id, playerNumber);
        CharacterControllers.Add(device_id, charControl);
        SetPlayerText(playerNumber);
        LinkSliderManager(charControl, playerNumber);
        LinkResourceDisplayManager(charControl, playerNumber);
        var data =
         new {
             type = "setup",
             value = playerNumber
        };
        AirConsole.instance.Message(device_id, data);
        playerNumber++;
        if(playerNumber > 3)
        {
            StartGame();
        }
    }

    private void LinkResourceDisplayManager(CharacterData charControl, int playerNumber)
    {
        charControl.OnResourceUpdate += ResourceTextManagers[playerNumber].Character_ResourceUpdated;
    }

    private void LinkSliderManager(CharacterData charControl, int playerNumber)
    {
        charControl.OnEnergyUpdate += SliderManagers[playerNumber].Character_EnergyUpdate;
    }

    private void SetPlayerText(int playerNumber)
    {
        if (playerNumber.Equals(0))
        {
            playerOneText.text = $"Player One {Environment.NewLine} CONNECTED";
        }
        else if (playerNumber.Equals(1))
        {
            playerTwoText.text = $"Player Two {Environment.NewLine} CONNECTED";

        }
        else if (playerNumber.Equals(2))
        {
            playerThreeText.text = $"Player Three {Environment.NewLine} CONNECTED";

        }
        else if (playerNumber.Equals(3))
        {
            playerFourText.text = $"Player Four {Environment.NewLine} CONNECTED";

        }
    }

    private void StartGame()
    {
        Debug.Log("starting game");
        
        GameRunning = !GameRunning;
        foreach(var player in CharacterControllers.Values) {
            player.Active = GameRunning;
        }

        GameStartScreen.SetActive(false);
        timer.Active = true;
    }

    private void EndGame()
    {
        //Determine the winner, end the game inputs, put up the winner text!

        List<CharacterData> listOfPlayerControls = new List<CharacterData>();
        foreach(var cc in CharacterControllers.Values)
        {
            listOfPlayerControls.Add(cc);
        }
        var sorted = listOfPlayerControls.OrderByDescending(x => x.CurrentResources).ToList();
        var winner = sorted.First();

        // End Screen
        GameRunning = false;
        foreach (var player in CharacterControllers.Values)
        {
            player.Active = GameRunning;
        }
        VictoryScreen.SetActive(true);
        victorTextController.SetVictorText(winner.PlayerNumber);
    }

    private Vector2 GetPlayerSpawnPos(int playerNumber)
    {
        if (playerNumber.Equals(0))
        {
            return mapController.GetTopLeft();
        }
        else if (playerNumber.Equals(1))
        {
            return mapController.GetTopRight();
        }
        else if (playerNumber.Equals(2))
        {
            return mapController.GetBottomLeft();
        }
        else if (playerNumber.Equals(3))
        {
            return mapController.GetBottomRight();
        }
        else
        {
            return Vector2.zero;
        }
    }

    void OnMessage(int from, JToken data)
    {
        if (GameRunning)
        {
            var command = ParseCommand(from, data);
            if (command is CommandData commandData)
            {
                CharacterControllers[from].ExecuteCommand(commandData);
            }
        }
    }

    private object ParseCommand(int from , JToken data)
    {
        // Look For a Key inside the data
        if(data["data"] is JToken input)
        {
            if(input["key"] is JToken inputVal)
            {
                var value = (string)inputVal;
                if(input["pressed"] is JToken pressedVal)
                {
                    var pressed = (bool)pressedVal;
                    if (pressed)
                    {
                        return CommandData.GenerateNewCommand(from, value);
                    }
                }
            }
        }
        return null;
    }
}
