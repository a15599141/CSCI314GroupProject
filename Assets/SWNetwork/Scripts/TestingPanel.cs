using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingPanel : MonoBehaviour // class to control test events in testing panel
{
    public Lobby lobby; // lobby instance to control lobby events

    /*----UI of Manual test section----*/
    public InputField RegisterText;
    public InputField RoomText;
    public InputField MessageText;

    public Button RegisterValid, RegisterInvalid;
    public Button RoomValid, RoomInvalid;
    public Button MessageValid, MessageInvalid;

    public Button RunManual1Button;
    public Button RunManual2Button;
    public Button RunManual3Button;

    /*----UI of Auto test section----*/
    public Button Register5cases, Register10cases;
    public Button Room5cases, Room10cases;
    public Button Message5cases, Message10cases;

    public Button RunAuto1Button;
    public Button RunAuto2Button;
    public Button RunAuto3Button;

    /*----UI of test result section----*/
    public Text TestResult;

    /*----Attributes for testing panel----*/
    int length;
    bool useNum, useLow, useUpp, useSpe;
    string custom = "";

    // Start is called before the first frame update
    void Start()
    {
        ManualTestRunInitialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManualTestRunInitialize() // Initialize trigger events for the buttons in testing panel
    {
        // initially disable some buttons
        RegisterValid.interactable = false;
        RoomValid.interactable = false;
        MessageValid.interactable = false;
        Register5cases.interactable = false;
        Room5cases.interactable = false;
        Message5cases.interactable = false;

        RunManual1Button.onClick.AddListener(() => // add trigger events for run test buttons of Registration in manual test
        {
            string playerName = RegisterText.text;
            TestResult.text = "------Registration Manual Test------\r\n";
            TestResult.text += "Testing player name:\r\n";
            TestResult.text += playerName+ "\r\n\r\n";

            bool ExpectedPlayerNameIsValid = !RegisterValid.IsInteractable();
            bool PlayerNameIsValid = VerifyPlayerName(playerName);

            AppendManualTestResult(ExpectedPlayerNameIsValid, PlayerNameIsValid);// add test report to Tested Result panel
        });
        RunManual2Button.onClick.AddListener(() => // add trigger events for run test buttons of Room Creation in manual test
        {
            string roomName = RoomText.text;
            TestResult.text = "------Create Room Manual Test------\r\n";
            TestResult.text += "Testing room name:\r\n";
            TestResult.text += roomName + "\r\n\r\n";

            bool ExpectedRoomNameIsValid = !RoomValid.IsInteractable();
            bool RoomNameIsValid = VerifyRoomName(roomName);

            AppendManualTestResult(ExpectedRoomNameIsValid, RoomNameIsValid);// add test report to Tested Result panel
        });
        RunManual3Button.onClick.AddListener(() => //add trigger events for run test buttons of Send Message in manual test
        {
            string message = MessageText.text;
            TestResult.text = "------Message Manual Test------\r\n";
            TestResult.text += "Testing message:\r\n";
            TestResult.text += message + "\r\n\r\n";

            bool ExpectedMessageIsValid = !MessageValid.IsInteractable();
            bool MessageIsValid = VerifyMessage(message);

            AppendManualTestResult(ExpectedMessageIsValid, MessageIsValid); // add test report to Tested Result panel
        });
        RunAuto1Button.onClick.AddListener(() => //add trigger events for run test buttons of Registration in auto test
        {
            TestResult.text = ""; // clear Tested Result panel
            int caseCount = 0; //Initialize the number of random case generation

            if (!Register5cases.IsInteractable()) caseCount = 5; // determine the number of random case
            else caseCount = 10;

            AppendAutoTestResult(1, lobby.MaxCharOfPlayerName,caseCount);
        });
        RunAuto2Button.onClick.AddListener(() =>// add trigger events for run test buttons of Room Creation in auto test
        {
            TestResult.text = "";
            int caseCount = 0;
            if (!Room5cases.IsInteractable()) caseCount = 5;
            else caseCount = 10;

            AppendAutoTestResult(2, lobby.MaxCharOfRoomName, caseCount);
        });
        RunAuto3Button.onClick.AddListener(() =>// add trigger events for run test buttons of Send Message in auto test
        {
            TestResult.text = "";

            int caseCount = 0;
            if (!Message5cases.IsInteractable()) caseCount = 5;
            else caseCount = 10;

            AppendAutoTestResult(3, lobby.MaxCharOfMessage, caseCount);
        });
    }

    public bool VerifyPlayerName(string playerName) // method for verify player name
    {
        bool valid = true; //initialize validity
        if (playerName.Length == 0) // if player name is empty
        {
            valid = false;
            TestResult.text += "◆ Player name must not be empty.\r\n";
        }
        if (playerName.Length > lobby.MaxCharOfPlayerName)// if player name's length longer than defined max length
        {
            valid = false;
            TestResult.text += "◆ Player name must not longer than " + lobby.MaxCharOfPlayerName + " character.\r\n";
        }
        string spe = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~ "; // invalid char 
        for (int i = 0; i < playerName.Length; i++) // traversal each char in playerName
        {
            if (spe.Contains(playerName[i].ToString())) // if the char is invalid
            {
                valid = false; 
                TestResult.text += "◆ Illegal character '" + playerName[i] + "' detected\r\n"; // add to test report
            }
        }
        if (!valid) TestResult.text += "\r\n";
        return valid;
    }
    public bool VerifyRoomName(string roomName)// method for verify room name
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
        if (!valid) TestResult.text += "\r\n";
        return valid;
    }
    public bool VerifyMessage(string message) // method for verify message
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
        if(!valid) TestResult.text += "\r\n";
        return valid;
    }
    public void AppendManualTestResult(bool ExpectedIsValid, bool TestedIsValid) // add manual test result to Tested Result panel
    {
        if (ExpectedIsValid && TestedIsValid)
        {
            TestResult.text += "Tested reslut: Valid\r\n";
            TestResult.text += "Expected reslut: Valid\r\n";
            TestResult.text += "Test Passed !\r\n";
        }
        else if (!ExpectedIsValid && !TestedIsValid)
        {
            TestResult.text += "Tested reslut: Invalid\r\n";
            TestResult.text += "Expected reslut: Invalid\r\n";
            TestResult.text += "Test Passed !\r\n";
        }
        else
        {
            if (TestedIsValid) TestResult.text += "Tested reslut: Valid\r\n";
            else TestResult.text += "Tested reslut: Invalid\r\n";
            if (ExpectedIsValid) TestResult.text += "Expected reslut: Valid\r\n";
            else TestResult.text += "Expected reslut: Invalid\r\n";
            TestResult.text += "Test Failed !\r\n";
        }
    }
    public void AppendAutoTestResult(int testType, int maxLength, int caseCount)// add auto test result to Tested Result panel
    {
        for (int i = 0; i < caseCount; i++)
        {
            int count = i + 1;
            string testTypeName="", testStringName="";

            if (testType == 1) 
            {
                testTypeName = "Registration";
                testStringName = "player name";
            }
            if (testType == 2)
            {
                testTypeName = "Room creation";
                testStringName = "room name";
            }
            if (testType == 3)
            {
                testTypeName = "Message Send";
                testStringName = "message";
            }
            TestResult.text += "-----" + testTypeName + " Auto Test " + count + "-----\r\n";

            length = Random.Range(0, maxLength * 2);
            useNum = Random.value > 0.5f;
            useLow = Random.value > 0.5f;
            useUpp = Random.value > 0.5f;
            useSpe = Random.value > 0.5f;

            string str = lobby.GetRandomString(length, useNum, useLow, useUpp, useSpe, custom);
            TestResult.text += "Testing "+ testStringName+":\r\n";
            TestResult.text += str + "\r\n";

            bool stringIsValid = true;
            if (testType == 1) stringIsValid=VerifyPlayerName(str);
            if (testType == 2) stringIsValid=VerifyRoomName(str);
            if (testType == 3) stringIsValid=VerifyMessage(str);

            TestResult.text += " \r\n\r\n";

            if (stringIsValid) 
            {
                TestResult.text += "Tested reslut: Valid\r\n";
                TestResult.text += testTypeName + " Passed !\r\n";
            }
            else
            {
                TestResult.text += "Tested reslut: Invalid\r\n";
                TestResult.text += testTypeName + "Failed !\r\n";
            }
        }
    }
}
