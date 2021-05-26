using System.Collections.Generic;
using Random = System.Random;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SWNetwork;
using TMPro;

public class Lobby : MonoBehaviour // lobby class to control lobby events
{
    public LobbyGUI GUI;  // LobbyGUI class instance to control the UI component's functions

    Dictionary<string, string> playersDict_; // Used to display players in different teams.
    RoomCustomData roomData_; // Current room's custom data.

    string playerName_; // Player entered name
    int currentRoomPageIndex_ = 0;// Current page index of the room list. 
    int MAX_PLAYER_NUM = 4; // maxiumn number of player that can join in a room

    /*---- UI of Lobby Entry----*/
    public Canvas LobbyCanvas; // lobby entry canvas 
    public InputField customPlayerIdField; // InputField for entering player name
    public Button playerNotice; // player registration notice (a question icon beside the registration InputField)
    public Text EntryRegisterText;// text of Register Button
    public TMP_Text LobbyEntryTitle; // Game lobby title in lobby entry
    public Button EntryRegisterButton;// Button for player registration 
    public Button ExitButton;   // button to exit the appliaction

    /*----Attributes for testing tool----*/
    public int MaxCharOfPlayerName = 10; // max char of player name
    public int MaxCharOfRoomName = 20; // max char of room name
    public int MaxCharOfMessage = 160; // max char of message
    int length; // length of the random generated string 
    bool useNum, useLow, useUpp, useSpe; // specify whether the random generated string contains number, lower case, upper case and special symbol
    string custom = " "; //a custom string that random generated string contians

    /*----UI of testing tool----*/
    public Button AutoRegistrationButton; // auto registration button
    public Button AutoCreateRoomButton;   // auto room creation button
    public Button AutoSendMessageButton;  // auto message send button

    public GameObject ExecutionPassed; // a green tick icon when execution pass
    public GameObject ExecutionFailed; // a red cross icon when execution pass
    public GameObject Executing; // Executing text when auto execution processing
    public Text AutoExecutionResultText; // test report 

    // Start method that call once in initialization
    void Start()
    {
        // nework object for Lobby events
        NetworkClient.Lobby.OnNewPlayerJoinRoomEvent += Lobby_OnNewPlayerJoinRoomEvent;
        NetworkClient.Lobby.OnPlayerLeaveRoomEvent += Lobby_OnPlayerLeaveRoomEvent;
        NetworkClient.Lobby.OnRoomCustomDataChangeEvent += Lobby_OnRoomCustomDataChangeEvent;
        NetworkClient.Lobby.OnRoomMessageEvent += Lobby_OnRoomMessageEvent;
        NetworkClient.Lobby.OnPlayerMessageEvent += Lobby_OnPlayerMessageEvent;
        NetworkClient.Lobby.OnLobbyConnectedEvent += Lobby_OnLobbyConncetedEvent;
        // allow player to register in Lobby Entry
        LobbyCanvas.GetComponent<CanvasGroup>().alpha = 0; // set lobby UI to invisible in beginning
        AutoExecutionInitialize(); // initialize the buttons of auto execution panel
    }

    // Update method that call every frame
    void Update()
    {
        GUI.LobbyPing.text = "ping: " + NetworkClient.Instance.LobbyPing+ "ms";
        if (Input.GetKeyDown(KeyCode.KeypadEnter)||Input.GetKeyDown(KeyCode.Return)) // allow user press enter to send message
        {
            SendRoomMessage(); // send room message method
        }
    }
    void OnDestroy()
    {
        // Unsubscrible to Lobby events
        NetworkClient.Lobby.OnNewPlayerJoinRoomEvent -= Lobby_OnNewPlayerJoinRoomEvent;
        NetworkClient.Lobby.OnPlayerLeaveRoomEvent -= Lobby_OnPlayerLeaveRoomEvent;
        NetworkClient.Lobby.OnRoomCustomDataChangeEvent -= Lobby_OnRoomCustomDataChangeEvent;
        NetworkClient.Lobby.OnRoomMessageEvent -= Lobby_OnRoomMessageEvent;
        NetworkClient.Lobby.OnPlayerMessageEvent -= Lobby_OnPlayerMessageEvent;
        NetworkClient.Lobby.OnLobbyConnectedEvent -= Lobby_OnLobbyConncetedEvent;
    }
    /* ----- Methods for auto execution tool ----- */
    public void AutoExecutionInitialize() // method to add trigger events on button when click
    {
        AutoCreateRoomButton.interactable = false; // set auto create room button into disable in beginning
        AutoSendMessageButton.interactable = false; // set auto send message button into disable in beginning
        AutoRegistrationButton.onClick.AddListener(() => // add trigger events when auto registration button click
        {
            AutoExecutionResultText.text = ""; // clear test report
            length = UnityEngine.Random.Range(0, MaxCharOfPlayerName*2); // random length of string
            useNum = UnityEngine.Random.value > 0.3f; // chance of having number in random generated string
            useLow = UnityEngine.Random.value > 0.3f; // chance of having loer case letter in random generated string
            useUpp = UnityEngine.Random.value > 0.3f; // chance of having upper case letter in random generated string
            useSpe = UnityEngine.Random.value > 0.5f; // chance of having special symbol in random generated string 
            customPlayerIdField.text = GetRandomString(length, useNum, useLow, useUpp, useSpe, custom); // add random generated string to registration input text field
            RegisterInLobbyEntry(); // process registration
            
        });
        AutoCreateRoomButton.onClick.AddListener(() => // add trigger events when auto create room button click
        {
            AutoExecutionResultText.text = ""; 
            length = UnityEngine.Random.Range(0, MaxCharOfRoomName*2); 
            useNum = UnityEngine.Random.value > 0.3f;
            useLow = UnityEngine.Random.value > 0.3f;
            useUpp = UnityEngine.Random.value > 0.3f;
            useSpe = UnityEngine.Random.value > 0.5f;
            GUI.HandleCreateRoomErrorOk(); // auto click on OK button if create room error already pop up
            CreateNewRoom(); //auto click on create room button
            GUI.newRoomText.text = GetRandomString(length, useNum, useLow, useUpp, useSpe, custom); 
            GUI.HandleCreateRoomOK();  //auto click on create room OK button
        });
        AutoSendMessageButton.onClick.AddListener(() => // add trigger events when auto send message button click
        {
            AutoExecutionResultText.text = "";
            length = UnityEngine.Random.Range(0, MaxCharOfMessage * 2);
            useNum = UnityEngine.Random.value > 0.3f;
            useLow = UnityEngine.Random.value > 0.3f;
            useUpp = UnityEngine.Random.value > 0.3f;
            useSpe = UnityEngine.Random.value > 0.5f;
            GUI.HandleSendRoomMessageErrorOk(); // auto click on OK button if message send error already pop up
            GUI.messageRoomText.text = GetRandomString(length, useNum, useLow, useUpp, useSpe, custom);
            SendRoomMessage(); // auto click on send message button
        });
    }
    public string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)// method of generating random string
    {
        byte[] b = new byte[4];
        new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
        Random r = new Random(BitConverter.ToInt32(b, 0));
        string s = null, str = custom;
        if (useNum == true) { str += "0123456789"; } 
        if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
        if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
        if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~ "; }
        for (int i = 0; i < length; i++)
        {
            s += str.Substring(r.Next(0, str.Length - 1), 1);
        }
        return s;
    }
    
    /* ----- Methods for lobby events ----- */
    public void RegisterInLobbyEntry()// register method 
    {
        playerName_ = customPlayerIdField.text;
        EntryRegisterText.text = "Registering...";
        ExecutionFailed.SetActive(false);
        ExecutionPassed.SetActive(false);
        EntryRegisterButton.interactable = false;

        if (playerName_.Length == 0) 
        {
            AutoExecutionResultText.text += "Player name can't be empty\r\n";
            ExecutionFailed.SetActive(true);
            EntryRegisterText.text = "Register";
            EntryRegisterButton.interactable = true;
            customPlayerIdField.Select();
        }
        if (playerName_.Length > MaxCharOfPlayerName)
        {
            AutoExecutionResultText.text += "Player name must not longer than "+ MaxCharOfPlayerName +" character\r\n";
            ExecutionFailed.SetActive(true);
            EntryRegisterText.text = "Register";
            EntryRegisterButton.interactable = true;
            customPlayerIdField.Select();
        }

        string spe = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~ ";
        int speCount = 0;
        for (int i = 0; i < playerName_.Length; i++)
        {
            if (spe.Contains(playerName_[i].ToString()))
            {
                AutoExecutionResultText.text += "Illegal character '"+playerName_[i]+"' detected\r\n";
                ExecutionFailed.SetActive(true);
                EntryRegisterText.text = "Register";
                EntryRegisterButton.interactable = true;
                customPlayerIdField.Select();
                speCount++;
            }
        }
        if (playerName_.Length > 0 && playerName_.Length <= MaxCharOfPlayerName && speCount==0)
        {
            AutoExecutionResultText.text += "Player name verified\r\n";
            AutoRegistrationButton.interactable = false;
            // use the user entered playerId to check into SocketWeaver. Make sure the PlayerId is unique.
            NetworkClient.Instance.CheckIn(playerName_, (bool ok, string error) =>
            {
                if (ok)
                {
                    AutoExecutionResultText.text += "Registration succeed\r\n";
                }
                else
                {
                    ExecutionFailed.SetActive(true);
                    AutoExecutionResultText.text += "Network Error: Failed to connect to server\r\n";
                    EntryRegisterText.text = "Register";
                    EntryRegisterButton.interactable = true;
                    AutoRegistrationButton.interactable = true;
                    Debug.LogError("Check-in failed: " + error);
                }
            });
        }

    }
    public void OnRoomSelected(string roomId) // join room method in lobby (when click the room name on room list panel)
    {
        Debug.Log("OnRoomSelected: " + roomId);
        // Join the selected room
        NetworkClient.Lobby.JoinRoom(roomId, (successful, reply, error) =>
        {
             if (successful)
             {
                 Debug.Log("Joined room " + reply);
                 // refresh the player list
                 GetPlayersInCurrentRoom();
             }
             else
             {
                 GUI.ShowJoinRoomErrorPopup();
                 Debug.Log("Failed to Join room " + error);
             }
        });
    }
    public void GetPlayersInCurrentRoom() // get joined players in current room
    {
        NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
        {
            if (successful)
            {
                Debug.Log("Got players " + reply);

                // store the playerIds and player names in a dictionary.
                // The dictionary is later used to populate the player list.
                playersDict_ = new Dictionary<string, string>();
                foreach (SWPlayer player in reply.players)
                {
                    playersDict_[player.id] = player.GetCustomDataString();
                }
                // fetch the room custom data.
                GetRoomCustomData();
            }
            else
            {
                Debug.Log("Failed to get players " + error);
            }
        });
    }
    public void GetRoomCustomData() // refresh the player list in a room
    {
        NetworkClient.Lobby.GetRoomCustomData((successful, reply, error) =>
        {
            if (successful)
            {
                Debug.Log("Got room custom data " + reply);
                // Deserialize the room custom data.
                roomData_ = reply.GetCustomData<RoomCustomData>();
                if (roomData_ != null)
                {
                    RefreshPlayerList();
                }
            }
            else
            {
                Debug.Log("Failed to get room custom data " + error);
            }
        });
    }
    public void CreateNewRoom() // create new room in lobby
    {
        GUI.ShowNewRoomPopup((bool ok, string roomName) =>
        {
            if (ok)
            {
                roomData_ = new RoomCustomData();
                roomData_.name = roomName;
                roomData_.team1 = new TeamCustomData();
                roomData_.team2 = new TeamCustomData();
                roomData_.team3 = new TeamCustomData();
                roomData_.team4 = new TeamCustomData();
                roomData_.team1.players.Add(NetworkClient.Lobby.PlayerId);
                // use the serializable roomData_ object as room's custom data.
                NetworkClient.Lobby.CreateRoom(roomData_, true, MAX_PLAYER_NUM, (successful, reply, error) =>
                {
                    if (successful)
                    {
                        AutoExecutionResultText.text += "Room creation succeed \r\n";
                        ExecutionPassed.SetActive(true);
                        Debug.Log("Room created " + reply);
                        // refresh the room list
                        GetRooms();
                        // refresh the player list
                        GetPlayersInCurrentRoom();
                    }
                    else
                    {
                        AutoExecutionResultText.text += "Create room ERROR: Player already in a room \r\n";
                        ExecutionFailed.SetActive(true);
                        GUI.ShowCreateRoomErrorPopup();
                        Debug.Log("Failed to create room " + error);
                    }
                    GUI.CreatingRoomMessagePopup.SetActive(false);
                });
            }
        });
    }
    public void SendRoomMessage() // send message in room
    {
        ExecutionFailed.SetActive(false);
        ExecutionPassed.SetActive(false);
        string message = GUI.messageRoomText.text;
        if (message.Length == 0 || message.Length > MaxCharOfMessage)
        {
            if(message.Length == 0) AutoExecutionResultText.text += "Message can not be empty\r\n";
            else AutoExecutionResultText.text += "Message can't more than "+MaxCharOfMessage+" charactor\r\n";
            ExecutionFailed.SetActive(true);
            GUI.messageRoomText.Select();
        }
        else
        {
            NetworkClient.Lobby.MessageRoom(message, (bool successful, SWLobbyError error) =>
            {
                if (successful)
                {
                    ExecutionPassed.SetActive(true);
                    AutoExecutionResultText.text += "Message successfully sent\r\n";
                    Debug.Log("Sent room message");
                    string msg = "Sent to room: " + message;
                    GUI.AddRowForMessage(msg, null, null);
                }
                else
                {
                    ExecutionFailed.SetActive(true);
                    AutoExecutionResultText.text += "Player must join a room to send message\r\n";
                    GUI.ShowSendRoomMessageErrorPopup();
                    Debug.Log("Failed to send room message " + error);
                }
            });
        }

    }
    public void OnPlayerSelected(string playerId) // select player name to send private message
    {
        Debug.Log("OnPlayerSelected: " + playerId);

        // demonstrate player message API
        GUI.ShowMessagePlayerPopup(playerId, (bool ok, string targetPlayerId, string message) =>
        {
            if (ok)
            {
                Debug.Log("Send player message " + "playerId= " + " message= " + message);
                NetworkClient.Lobby.MessagePlayer(playerId, message, (bool successful, SWLobbyError error) =>
                {
                    if (successful)
                    {
                        Debug.Log("Sent player message");
                        string msg = "Sent to " + ": " + message;
                        GUI.AddRowForMessage(msg, null, null);
                    }
                    else
                    {
                        Debug.Log("Failed to send player messagem " + error);
                    }
                });
            }
        });
    }
    void RefreshPlayerList() // refresh the player list in a room
    {
        // Use the room custom data, and the playerId and player Name dictionary to populate the player lsit
        if (playersDict_ != null)
        {
            GUI.ClearPlayerList();
            GUI.AddRowForTeam("Player 1");
            foreach (string pId in roomData_.team1.players)
            {
                String playerName = playersDict_[pId];
                GUI.AddRowForPlayer(playerName, pId, OnPlayerSelected);
            }

            GUI.AddRowForTeam("Player 2");
            foreach (string pId in roomData_.team2.players)
            {
                String playerName = playersDict_[pId];
                GUI.AddRowForPlayer(playerName, pId, OnPlayerSelected);
            }

            GUI.AddRowForTeam("Player 3");
            foreach (string pId in roomData_.team3.players)
            {
                String playerName = playersDict_[pId];
                GUI.AddRowForPlayer(playerName, pId, OnPlayerSelected);
            }

            GUI.AddRowForTeam("Player 4");
            foreach (string pId in roomData_.team4.players)
            {
                String playerName = playersDict_[pId];
                GUI.AddRowForPlayer(playerName, pId, OnPlayerSelected);
            }
        }
    }
    public void GetRooms() // refresh room list in lobby
    {
        // Get the rooms for the current page.
        NetworkClient.Lobby.GetRooms(currentRoomPageIndex_, 6, (successful, reply, error) =>
        {
            if (successful)
            {
                Debug.Log("Got rooms " + reply);

                // Remove rooms in the rooms list
                GUI.ClearRoomList();

                foreach (SWRoom room in reply.rooms)
                {
                    Debug.Log(room);
                    // Deserialize the room custom data.
                    RoomCustomData rData = room.GetCustomData<RoomCustomData>();
                    if (rData != null)
                    {
                        // Add rooms to the rooms list.
                        GUI.AddRowForRoom(rData.name, room.id, OnRoomSelected);
                    }
                }
            }
            else
            {
                Debug.Log("Failed to get rooms " + error);
            }
        });
    }
    public void NextPage() // view next page of the room list
    {
        currentRoomPageIndex_++;
        GetRooms();
    }
    public void PreviousPage() // view previous page of the room list
    {
        currentRoomPageIndex_--;
        GetRooms();
    }
    public void LeaveRoom() // leave the current joined room 
    {
        NetworkClient.Lobby.LeaveRoom((successful, error) =>
        {
            if (successful)
            {
                Debug.Log("Left room");
                GUI.ClearPlayerList();
                GUI.ClearRoomMessage();
                GetRooms();
            }
            else
            {
                GUI.ShowLeaveRoomErrorPopup();
                Debug.Log("Failed to leave room " + error);
            }
        });
    }

    /* -----Method for lobby network events ----- */
    void Lobby_OnLobbyConncetedEvent() // called after check in 
    {
        Debug.Log("Lobby_OnLobbyConncetedEvent");
        // Register the player using the entered player name.
        NetworkClient.Lobby.Register(playerName_, (successful, reply, error) =>
        {
            if (successful)
            {
                AutoExecutionResultText.text += "Network connected\r\n";
                AutoExecutionResultText.text += "Login to lobby succeed\r\n";
                GUI.playerNameText.text = playerName_;
                LobbyEntryTitle.gameObject.SetActive(false);
                ExecutionPassed.SetActive(true);
                customPlayerIdField.gameObject.SetActive(false);
                EntryRegisterButton.gameObject.SetActive(false);
                ExitButton.gameObject.SetActive(false);
                playerNotice.gameObject.SetActive(false);
                LobbyCanvas.GetComponent<CanvasGroup>().alpha = 1;
                AutoCreateRoomButton.interactable = true;
                AutoSendMessageButton.interactable = true;
                Debug.Log("Lobby registered " + reply);
                GetRooms();
            }
            else
            {
                ExecutionFailed.SetActive(true);
                EntryRegisterButton.interactable = true;
                AutoRegistrationButton.interactable = true;
                AutoExecutionResultText.text += "Network Error: Failed to connect to server\r\n";
                EntryRegisterText.text = "Register";
                Debug.Log("Lobby failed to register " + error);
            }
        });
        
    } 
    void Lobby_OnNewPlayerJoinRoomEvent(SWJoinRoomEventData eventData) // called when player join room
    {
        Debug.Log("Player joined room");
        Debug.Log(eventData);
        // Store the new playerId and player name pair
        playersDict_[eventData.newPlayerId] = eventData.GetString();
        if (NetworkClient.Lobby.IsOwner)
        {
            // Find the team has space and assign the new player to it.
            if (roomData_.team2.players.Count < roomData_.team1.players.Count)
            {
                roomData_.team2.players.Add(eventData.newPlayerId);
            }
            else if (roomData_.team3.players.Count < roomData_.team2.players.Count)
            {
                roomData_.team3.players.Add(eventData.newPlayerId);
            }
            else if (roomData_.team4.players.Count < roomData_.team3.players.Count)
            {
                roomData_.team4.players.Add(eventData.newPlayerId);
            }
            else if (roomData_.team1.players.Count < roomData_.team4.players.Count)
            {
                roomData_.team1.players.Add(eventData.newPlayerId);
            }
            // Update the room custom data
            NetworkClient.Lobby.ChangeRoomCustomData(roomData_, (bool successful, SWLobbyError error) =>
            {
                if (successful)
                {
                    Debug.Log("ChangeRoomCustomData successful");
                    RefreshPlayerList();
                }
                else
                {
                    Debug.Log("ChangeRoomCustomData failed: " + error);
                }
            });
        }
    } 
    void Lobby_OnPlayerLeaveRoomEvent(SWLeaveRoomEventData eventData) // called when player leave room
    {
        Debug.Log("Player left room: " + eventData);

        if (NetworkClient.Lobby.IsOwner)
        {
            // Remove the players from both team.
            roomData_.team1.players.RemoveAll(eventData.leavePlayerIds.Contains);
            roomData_.team2.players.RemoveAll(eventData.leavePlayerIds.Contains);
            roomData_.team3.players.RemoveAll(eventData.leavePlayerIds.Contains);
            roomData_.team4.players.RemoveAll(eventData.leavePlayerIds.Contains);
            // Update the room custom data
            NetworkClient.Lobby.ChangeRoomCustomData(roomData_, (bool successful, SWLobbyError error) =>
            {
                if (successful)
                {
                    Debug.Log("ChangeRoomCustomData successful");
                    RefreshPlayerList();
                }
                else
                {
                    Debug.Log("ChangeRoomCustomData failed: " + error);
                }
            });
        }
    }
    void Lobby_OnRoomCustomDataChangeEvent(SWRoomCustomDataChangeEventData eventData) // called when player refresh room list
    {
        Debug.Log("Room custom data changed: " + eventData);

        SWRoom room = NetworkClient.Lobby.RoomData;
        roomData_ = room.GetCustomData<RoomCustomData>();

        // Room custom data changed, refresh the player list.
        RefreshPlayerList();
    }
    void Lobby_OnRoomMessageEvent(SWMessageRoomEventData eventData) // called when send rrom message
    {
        string msg = "Room message: " + eventData.data;
        GUI.AddRowForMessage(msg, null, null);
    }
    void Lobby_OnPlayerMessageEvent(SWMessagePlayerEventData eventData) // called when send message to selected player
    {
        string msg = eventData.playerId + ": " + eventData.data;
        GUI.AddRowForMessage(msg, null, null);
    }
    
    // method when click on Exit button
    public void Exit()
    {
        Application.Quit();
    }
    // method when click on back button in testing panel
    public void TestingPanelBack()
    {
        LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }
}
