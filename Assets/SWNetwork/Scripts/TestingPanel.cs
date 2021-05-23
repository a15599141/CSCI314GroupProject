using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingPanel : MonoBehaviour
{
    public Lobby lobby;
    public InputField RegisterText;
    public InputField RoomText;
    public InputField MessageText;

    public Button RegisterValid, RegisterInvalid;
    public Button RoomValid, RoomInvalid;
    public Button MessageValid, MessageInvalid;

    public Button Register5cases, Register10cases;
    public Button Room5cases, Room10cases;
    public Button Message5cases, Message10cases;

    public Button RunManual1Button, RunAuto1Button;
    public Button RunManual2Button, RunAuto2Button;
    public Button RunManual3Button, RunAuto3Button;
    public Text TestResult;

    int length;
    bool useNum, useLow, useUpp, useSpe;
    string custom = " ";
    // Start is called before the first frame update
    void Start()
    {
        ManualTestRunInitialize();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManualTestRunInitialize()
    {
        RegisterValid.interactable = false;
        RoomValid.interactable = false;
        MessageValid.interactable = false;
        Register5cases.interactable = false;
        Room5cases.interactable = false;
        Message5cases.interactable = false;
        RunManual1Button.onClick.AddListener(() =>
        {
            TestResult.text = "------Registration Manual Test------\r\n";
            string playerName = RegisterText.text;
            TestResult.text += "Testing player name:\r\n";
            TestResult.text += playerName+ "\r\n";
            bool PlayerNameIsValid = VerifyPlayerName(playerName);
            bool ExpectedPlayerNameIsValid = !RegisterValid.IsInteractable();
            TestResult.text += " \r\n";
            if (ExpectedPlayerNameIsValid && PlayerNameIsValid)
            {
                TestResult.text += "Tested reslut: Valid\r\n";
                TestResult.text += "Expected reslut: Valid\r\n";
                TestResult.text += "Test Passed !\r\n";
            }
            else if(!ExpectedPlayerNameIsValid && !PlayerNameIsValid)
            {
                TestResult.text += "Tested reslut: Invalid\r\n";
                TestResult.text += "Expected reslut: Invalid\r\n";
                TestResult.text += "Test Passed !\r\n";
            }
            else
            {
                if(PlayerNameIsValid) TestResult.text += "Tested reslut: Valid\r\n";
                else TestResult.text += "Tested reslut: Invalid\r\n";
                if(ExpectedPlayerNameIsValid) TestResult.text += "Expected reslut: Valid\r\n";
                else TestResult.text += "Expected reslut: Invalid\r\n";
                TestResult.text += "Test Failed !\r\n";
            }
        });
        RunManual2Button.onClick.AddListener(() =>
        {
            TestResult.text = "------Create Room Manual Test------\r\n";
            string roomName = RoomText.text;
            TestResult.text += "Testing room name:\r\n";
            TestResult.text += roomName + "\r\n";
            bool RoomNameIsValid = VerifyRoomName(roomName);
            bool ExpectedRoomNameIsValid = !RoomValid.IsInteractable();
            TestResult.text += " \r\n";
            if (ExpectedRoomNameIsValid && RoomNameIsValid)
            {
                TestResult.text += "Tested reslut: Valid\r\n";
                TestResult.text += "Expected reslut: Valid\r\n";
                TestResult.text += "Test Passed !\r\n";
            }
            else if (!ExpectedRoomNameIsValid && !RoomNameIsValid)
            {
                TestResult.text += "Tested reslut: Invalid\r\n";
                TestResult.text += "Expected reslut: Invalid\r\n";
                TestResult.text += "Test Passed !\r\n";
            }
            else
            {
                if (RoomNameIsValid) TestResult.text += "Tested reslut: Valid\r\n";
                else TestResult.text += "Tested reslut: Invalid\r\n";
                if (ExpectedRoomNameIsValid) TestResult.text += "Expected reslut: Valid\r\n";
                else TestResult.text += "Expected reslut: Invalid\r\n";
                TestResult.text += "Test Failed !\r\n";
            }
        });
        RunManual3Button.onClick.AddListener(() =>
        {
            TestResult.text = "------Message Manual Test------\r\n";
            string message = MessageText.text;
            TestResult.text += "Testing message:\r\n";
            TestResult.text += message + "\r\n";
            bool MessageIsValid = VerifyMessage(message);
            bool ExpectedMessageIsValid = !MessageValid.IsInteractable();
            TestResult.text += " \r\n";
            if (ExpectedMessageIsValid && MessageIsValid)
            {
                TestResult.text += "Tested reslut: Valid\r\n";
                TestResult.text += "Expected reslut: Valid\r\n";
                TestResult.text += "Test Passed !\r\n";
            }
            else if (!ExpectedMessageIsValid && !MessageIsValid)
            {
                TestResult.text += "Tested reslut: Invalid\r\n";
                TestResult.text += "Expected reslut: Invalid\r\n";
                TestResult.text += "Test Passed !\r\n";
            }
            else
            {
                if (MessageIsValid) TestResult.text += "Tested reslut: Valid\r\n";
                else TestResult.text += "Tested reslut: Invalid\r\n";
                if (ExpectedMessageIsValid) TestResult.text += "Expected reslut: Valid\r\n";
                else TestResult.text += "Expected reslut: Invalid\r\n";
                TestResult.text += "Test Failed !\r\n";
            }
        });
        RunAuto1Button.onClick.AddListener(() =>
        {
            TestResult.text = "";
            int caseCount = 0;
            if (!Register5cases.IsInteractable()) caseCount = 5;
            else caseCount = 10;
            for (int i = 0; i < caseCount; i++)
            {
                int count = i + 1;
                TestResult.text += "-----Registration Auto Test "+ count + "-----\r\n";
                length = Random.Range(0, lobby.MaxCharOfPlayerName * 2);
                useNum = Random.value > 0.5f;
                useLow = Random.value > 0.5f;
                useUpp = Random.value > 0.5f;
                useSpe = Random.value > 0.5f;
                string playerName;
                if(length==0) playerName = "";
                else playerName = lobby.GetRandomString(length, useNum, useLow, useUpp, useSpe, custom);
                TestResult.text += "Testing player name:\r\n";
                TestResult.text += playerName + "\r\n";
                bool playerNameIsValid = VerifyPlayerName(playerName);
                TestResult.text += " \r\n";
                if (playerNameIsValid)
                {
                    TestResult.text += "Tested reslut: Valid\r\n";
                    TestResult.text += "Registration Passed !\r\n";
                }
                else
                {
                    TestResult.text += "Tested reslut: Invalid\r\n";
                    TestResult.text += "Registration Failed !\r\n";
                }
            }
        });
        RunAuto2Button.onClick.AddListener(() =>
        {
            TestResult.text = "";
            int caseCount = 0;
            if (!Room5cases.IsInteractable()) caseCount = 5;
            else caseCount = 10;
            for (int i = 0; i < caseCount; i++)
            {
                int count = i + 1;
                TestResult.text += "-----Room creation Auto Test " + count + "-----\r\n";
                length = Random.Range(0, lobby.MaxCharOfRoomName * 2);
                useNum = Random.value > 0.5f;
                useLow = Random.value > 0.5f;
                useUpp = Random.value > 0.5f;
                useSpe = Random.value > 0.5f;
                string roomName;
                if (length == 0) roomName = "";
                else roomName = lobby.GetRandomString(length, useNum, useLow, useUpp, useSpe, custom);
                TestResult.text += "Testing room name:\r\n";
                TestResult.text += roomName + "\r\n";
                bool roomNameIsValid = VerifyRoomName(roomName);
                TestResult.text += " \r\n";
                if (roomNameIsValid)
                {
                    TestResult.text += "Tested reslut: Valid\r\n";
                    TestResult.text += "Room Creation Passed !\r\n";
                }
                else
                {
                    TestResult.text += "Tested reslut: Invalid\r\n";
                    TestResult.text += "Room Creation Failed !\r\n";
                }
            }
        });
        RunAuto3Button.onClick.AddListener(() =>
        {
            TestResult.text = "";
            int caseCount = 0;
            if (!Message5cases.IsInteractable()) caseCount = 5;
            else caseCount = 10;
            for (int i = 0; i < caseCount; i++)
            {
                int count = i + 1;
                TestResult.text += "-----Message Send Auto Test " + count + "-----\r\n";
                length = Random.Range(0, lobby.MaxCharOfMessage * 2);
                useNum = Random.value > 0.6f;
                useLow = Random.value > 0.4f;
                useUpp = Random.value > 0.4f;
                useSpe = Random.value > 0.4f;
                string message;
                if (length == 0) message = "";
                else message = lobby.GetRandomMessage(length, useNum, useLow, useUpp, useSpe, custom);
                TestResult.text += "Testing message:\r\n";
                TestResult.text += message + "\r\n";
                bool messageIsValid = VerifyMessage(message);
                TestResult.text += " \r\n";
                if (messageIsValid)
                {
                    TestResult.text += "Tested reslut: Valid\r\n";
                    TestResult.text += "Message Send Passed !\r\n";
                }
                else
                {
                    TestResult.text += "Tested reslut: Invalid\r\n";
                    TestResult.text += "Message Send Failed !\r\n";
                }
            }
        });
    }

    public bool VerifyPlayerName(string playerName)
    {
        bool valid = true;
        if (playerName.Length == 0)
        {
            valid = false;
            TestResult.text += "◆ Player name must not be empty.\r\n";
        }
        if (playerName.Length > lobby.MaxCharOfPlayerName)
        {
            valid = false;
            TestResult.text += "◆ Player name must not longer than " + lobby.MaxCharOfPlayerName + " character.\r\n";
        }
        string spe = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~ ";
        for (int i = 0; i < playerName.Length; i++)
        {
            if (spe.Contains(playerName[i].ToString()))
            {
                valid = false;
                TestResult.text += "◆ Illegal character '" + playerName[i] + "' detected\r\n";
            }
        }
        return valid;
    }
    public bool VerifyRoomName(string roomName)
    {
        bool valid = true;
        if (roomName.Length == 0)
        {
            valid = false;
            TestResult.text += "◆ Room name must not be empty.\r\n";
        }
        if (roomName.Length > lobby.MaxCharOfRoomName)
        {
            valid = false;
            TestResult.text += "◆ Room name must not longer than " + lobby.MaxCharOfRoomName + " character.\r\n";
        }
        string spe = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~ ";
        for (int i = 0; i < roomName.Length; i++)
        {
            if (spe.Contains(roomName[i].ToString()))
            {
                valid = false;
                TestResult.text += "◆ Illegal character '" + roomName[i] + "' detected\r\n";
            }
        }
        return valid;
    }
    public bool VerifyMessage(string message)
    {
        bool valid = true;
        if (message.Length == 0)
        {
            valid = false;
            TestResult.text += "◆ Message must not be empty.\r\n";
        }
        if (message.Length > lobby.MaxCharOfMessage)
        {
            valid = false;
            TestResult.text += "◆ Message must not longer than " + lobby.MaxCharOfMessage + " character.\r\n";
        }
        /*string spe = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~ ";
        for (int i = 0; i < message.Length; i++)
        {
            if (spe.Contains(message[i].ToString()))
            {
                valid = false;
                TestResult.text += "◆ Illegal character '" + message[i] + "' detected\r\n";
            }
        }*/
        return valid;
    }
    
}
