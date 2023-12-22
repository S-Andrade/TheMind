using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


public enum GameState
{
    Connection,
    Syncing,
    Game,
    Mistake,
    NextLevel,
    GameFinished,
    UseStar,
    WaitingAnswer,
    Wait
}


public class GameManager : MonoBehaviour
{

    public GameObject ConfigsScreen;
    public GameObject IpInputField;
    public GameObject PortInputField;
    public GameObject PlayerIDInputField;
    public GameObject ConnectButton;
    public GameObject GameScreen;
    public GameObject PlayButtonUI;
    public GameObject RefocusButtonUI;
    public GameObject ReadyButtonUI;
    public GameObject CardsUI;

    public GameObject StarButtonUI;
    public GameObject YesStarButtonUI;
    public GameObject NoStarButtonUI;

    private int ID;
    private List<int> cards;
    private bool HasSignalledRefocus;
    private bool IsReady;
    private bool ShouldAckMistake;
    private bool endlevel;
    private int level;

    private String AgreeStar;

    private bool HasSignalledUseStar;

    public int MaxLevel;
    private TabletThalamusConnector _thalamusConnector;

    public static GameState GameState;
    public static GameState PreviuosGameState;
    public int Stars;

    // Start is called before the first frame update
    void Start()
    {
        GameState = GameState.Connection;
        HasSignalledRefocus = false;
        IsReady = false;
        ShouldAckMistake = false;
        HasSignalledUseStar = false;
        AgreeStar = null;
        endlevel = false;
        level = 1;
        ConfigsScreen.SetActive(true);
        PortInputField.GetComponent<InputField>().text = "7030";
        PlayerIDInputField.GetComponent<InputField>().text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Debug.Log(GameState);
        if (GameState != GameState.Connection)
        {
            GameScreen.SetActive(true);
            UpdateCardsUI();
            UpdatePlayButtonUI();
            UpdateRefocusButtonUI();
            UpdateReadyButtonUI();
            UpdateStarButtonUI();
            UpdateAnswerStarButtonUI();
        }
        
        if (GameState == GameState.GameFinished)
        {
            Application.Quit();
        }
    }
    void UpdateAnswerStarButtonUI()
    {
        if (GameState == GameState.UseStar)
        {
            YesStarButtonUI.GetComponent<Button>().interactable = true;
            YesStarButtonUI.GetComponent<Image>().enabled = true;
            //YesStarButtonUI.GetComponentInChildren<Text>().text = "Yes";

            NoStarButtonUI.GetComponent<Button>().interactable = true;
            NoStarButtonUI.GetComponent<Image>().enabled = true;
            //NoStarButtonUI.GetComponentInChildren<Text>().text = "No";
        
        }
        else
        {
            YesStarButtonUI.GetComponent<Button>().interactable = false;
            YesStarButtonUI.GetComponent<Image>().enabled = false;

            NoStarButtonUI.GetComponent<Button>().interactable = false;
            NoStarButtonUI.GetComponent<Image>().enabled = false;
         
        }
    }
    void UpdateStarButtonUI()
    {
        if (GameState == GameState.Game && cards.Count > 0 && Stars > 0)
        {
            StarButtonUI.GetComponent<Button>().interactable = true;
            StarButtonUI.GetComponentInChildren<Text>().text = "Star";
        }
        else
        {
            StarButtonUI.GetComponent<Button>().interactable = false;
        }
    }
    void UpdateReadyButtonUI()
    {

        if (GameState == GameState.NextLevel && !IsReady)
        {
            ReadyButtonUI.GetComponent<Button>().interactable = true;
            ReadyButtonUI.GetComponentInChildren<Text>().text = "Ready";
        }
        else if (GameState == GameState.Mistake && (cards.Count > 0 || ShouldAckMistake) && !IsReady)
        {
            ReadyButtonUI.GetComponent<Button>().interactable = true;
            ReadyButtonUI.GetComponentInChildren<Text>().text = "Continue";
        }
        else
        {
            ReadyButtonUI.GetComponent<Button>().interactable = false;
        }
    }
    void UpdatePlayButtonUI()
    {

        if (GameState == GameState.Game && cards.Count > 0)
        {
            PlayButtonUI.GetComponent<Button>().interactable = true;
        }
        else
        {
            PlayButtonUI.GetComponent<Button>().interactable = false;
        }
    }
    void UpdateRefocusButtonUI()
    {
        if (GameState == GameState.NextLevel || GameState == GameState.Mistake || GameState == GameState.UseStar || GameState == GameState.WaitingAnswer || GameState == GameState.Wait)
        {
            RefocusButtonUI.GetComponent<Button>().interactable = false;
        }
        else if (cards.Count == 0 || (GameState == GameState.Syncing && HasSignalledRefocus))
        {
            RefocusButtonUI.GetComponent<Button>().interactable = false;
        }
        else
        {
            RefocusButtonUI.GetComponent<Button>().interactable = true;
        }
    }
    void UpdateCardsUI()
    {
        if (GameState == GameState.Syncing || GameState == GameState.Game || GameState == GameState.Mistake || GameState == GameState.Wait)
        {
            string text = "[";
            for (int i = 0; i < cards.Count; i++)
            {
                text += cards[i];
                if (i != cards.Count - 1)
                {
                    text += ",";
                }
            }
            text += "]";
            CardsUI.GetComponent<Text>().text = text;

        }
    }
    public void AllPlayersRefocused()
    {
        GameState = GameState.Game;
        HasSignalledRefocus = false;
        IsReady = false;
        ShouldAckMistake = false;
    }
    public void ConnectOnClick()
    {
        ConfigsScreen.SetActive(false);
        ID = int.Parse(PlayerIDInputField.GetComponent<InputField>().text);
        string IP = IpInputField.GetComponent<InputField>().text;
        int port = int.Parse(PortInputField.GetComponent<InputField>().text);
        _thalamusConnector = new TabletThalamusConnector(this, IP, port);
        _thalamusConnector.ConnectToGM(ID, "Tablet" + (ID + 1));
    }
    public void WaitForNewLevel()
    {
        IsReady = false;
        ShouldAckMistake = false;
        GameState = GameState.NextLevel;
        
    }
    public void GameFinished()
    {
        //endlevel = true;
        GameState = GameState.GameFinished;
        //Application.Quit();
    }
    public void NewLevelHasStarted(int level, int stars, int[] p0Hand, int[] p1Hand, int[] p2Hand)
    {
        level = level;
        UnityEngine.Debug.Log("NewLevel");
        endlevel = false;
        Stars = stars;
        UnityEngine.Debug.Log(Stars);
        cards = new List<int>();
        if (ID == 0)
        {
            foreach (int card in p0Hand)
            {
                cards.Add(card);
            }
        }
        else if (ID == 1)
        {
            foreach (int card in p1Hand)
            {
                cards.Add(card);
            }
        }
        else if (ID ==  2)        {
            foreach (int card in p2Hand)
            {
                cards.Add(card);


            }
        }
        GameState = GameState.Syncing;
    }
    public void PlayerRequestedRefocus(int playerID)
    {
        if (playerID == -1)
        {
            GameState = GameState.Game;
        }
        else
        {
            GameState = GameState.Syncing;
        }
    }
    public void MistakeOccurred(int playerID, int card, int[] p0WrongCards, int[] p1WrongCards, int[] p2WrongCards)
    {
        GameState = GameState.Mistake;
        //mistake by another player
        if (playerID != ID)
        {
            if (ID == 0)
            {
                foreach (int wrongCard in p0WrongCards)
                {
                    ShouldAckMistake = true;
                    cards.Remove(wrongCard);
                }
            }
            else if (ID == 1)
            {
                foreach (int wrongCard in p1WrongCards)
                {
                    ShouldAckMistake = true;
                    cards.Remove(wrongCard);
                }
            }
            else if (ID == 2)
            {
                foreach (int wrongCard in p2WrongCards)
                {
                    ShouldAckMistake = true;
                    cards.Remove(wrongCard);
                }
            }
        }
    }
    public void AllAgreeStar()
    {
        cards.RemoveAt(0);
        GameState = GameState.Syncing;
        HasSignalledUseStar = false;
        AgreeStar = null;
        Stars--;
    }
    public void NotAllAgreeStar()
    {
        GameState = GameState.Syncing;
        HasSignalledUseStar = false;
        AgreeStar = null;
        UnityEngine.Debug.Log("NotAll");
    }
    public void PlayerRequestedStar(int playerID)
    {
        if (playerID == -1)
        {
            GameState = GameState.Game;
        }
        else
        {
            GameState = GameState.UseStar;
        }
    }
    public void PlayButton()
    {
        int cardToPlay = cards[0];
        cards.RemoveAt(0);
        _thalamusConnector.PlayCard(ID, cardToPlay);
    }
    public void RefocusButton()
    {
        HasSignalledRefocus = true;
        _thalamusConnector.RefocusSignal(ID);
    }
    public void ReadyButton()
    {
        IsReady = true;
        if (GameState == GameState.NextLevel)
        {
            _thalamusConnector.ReadyForNextLevel(ID);
        }
        else if (GameState == GameState.Mistake)
        {
            _thalamusConnector.ContinueAfterMistake(ID);
        }
    }
    public void StarButton()
    {
        GameState = GameState.UseStar;
        HasSignalledUseStar = true;
        _thalamusConnector.StarSignal(ID);
    }
    public void NoStarButton()
    {
        AgreeStar = "NO";
        _thalamusConnector.NoStarSignal(ID);
        GameState = GameState.WaitingAnswer;
        UnityEngine.Debug.Log(AgreeStar);
    }
    public void YesStarButton()
    {
        AgreeStar = "YES";
        _thalamusConnector.YesStarSignal(ID);
        GameState = GameState.WaitingAnswer;
        UnityEngine.Debug.Log(AgreeStar);
    }
    public void StartWait()
    {
        //PreviuosGameState = GameState;
        GameState = GameState.Wait;
        UnityEngine.Debug.Log("startWait");
    }
    public void EndWait()
    {
        if (cards.Count == 0)
        {
            GameState = GameState.NextLevel;
        }
        else
        {
            GameState = GameState.Game;
        }
        UnityEngine.Debug.Log("EndWait");
    }

}
