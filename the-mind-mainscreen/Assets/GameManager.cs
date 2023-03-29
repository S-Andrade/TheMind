using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using UnityEditor.Timeline;
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
    UseStar

}

public class GameManager : MonoBehaviour
{
    private string IPadress;
    public int MaxLevels;
    public int NumPlayers;
    public int Level;
    public GameObject LevelUI;
    public int Lives;
    public GameObject LivesUI;
    public int Stars;
    public GameObject StarsUI;
    public GameObject OverlayNextLevelUI;
    public GameObject OverlaySyncingUI;
    public GameObject OverlayMistakeUI;
    public GameObject OverlayStarUI;
    public GameObject GameFinishedTextUI;
    public GameObject MaxLevelsInputFieldUI;
    public GameObject IPinputField;
    public static bool DebugMode = true;
    public static GameState GameState;
    public AudioClip playingCardSound;
    public AudioClip errorSound;
    private AudioSource audioSource;

    public int points;
    public int ngames;

    public Player[] players;
    public Pile pile;
    private int topOfThePile;
    public static int CONDITION;
    private int[][][] cardsPerLevelPerCondition = new int[][][] {
        new int[][] {
            new int[] { 39, 6, 17 },
            new int[] { 12, 23, 14, 36, 10, 48 },
            new int[] { 7, 32, 45, 9, 29, 41, 11, 20, 43 }
        },
        new int[][] {
            new int[] { 40, 7, 18 },
            new int[] { 13, 24, 15, 37, 11, 49 },
            new int[] { 8, 33, 46, 10, 30, 42, 12, 21, 44 }
        },
        new int[][] {
            new int[] { 38, 5, 16 },
            new int[] { 11, 22, 13, 35, 9, 47 },
            new int[] { 6, 31, 44, 8, 28, 40, 10, 19, 42 }
        }
    };

    private GameMasterThalamusConnector _thalamusConnector;

    // Start is called before the first frame update
    void Start()
    {
        IPadress = "";
        topOfThePile = -1;
        _thalamusConnector = null;
        GameState = GameState.Connection;
        audioSource = GetComponent<AudioSource>();
        //CONDITION = -1;
        points = 0;
        ngames = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_thalamusConnector == null && IPadress != "")
        {
            IPinputField.GetComponent<InputField>().interactable = false;
            _thalamusConnector = new GameMasterThalamusConnector(this, IPadress);
        }
        UpdateNumLevelsSetupUI();
        if (GameState == GameState.Connection)
        {
            if (players[0].IsConnected && players[1].IsConnected && players[2].IsConnected)
            {
                IPinputField.SetActive(false);
                _thalamusConnector.AllConnected(MaxLevels, players[0].ID, players[0].Name, players[1].ID, players[1].Name, players[2].ID, players[2].Name);
                OverlayNextLevelUI.SetActive(true);
                GameState = GameState.NextLevel;
            }
        }

        if (GameState == GameState.Syncing)
        {
            OverlaySyncingUI.SetActive(true);
            if ((players[0].HasSignaledRefocus || players[0].HowManyCardsLeft() == 0) && (players[1].HasSignaledRefocus || players[1].HowManyCardsLeft() == 0) && (players[2].HasSignaledRefocus || players[2].HowManyCardsLeft() == 0))
            {
                _thalamusConnector.AllRefocused();
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].HasSignaledRefocus = false;
                }
                InvokeRepeating("ShrinkUntilDeactiveSyncingUI", 0, 0.006f);
                GameState = GameState.Game;
            }
        }

        if (GameState == GameState.Game)
        {
            int updatedTopOfThePile = pile.GetTopCard();
            if (topOfThePile != updatedTopOfThePile)
            {
                topOfThePile = updatedTopOfThePile;
                ValidateMove();
            }
            else if (players[0].HowManyCardsLeft() == 0 && players[1].HowManyCardsLeft() == 0 && players[2].HowManyCardsLeft() == 0)
            {
                _thalamusConnector.FinishLevel(Level, Lives);
                if (Level == MaxLevels)
                {
                    OverlayMistakeUI.SetActive(true);
                    GameFinishedTextUI.SetActive(true);
                    GameFinishedTextUI.GetComponent<Text>().text = "Game Completed!";
                    GameState = GameState.GameFinished;
                    points += Level;
                    ngames++;
                    _thalamusConnector.GameCompleted();
                }
                else
                {
                    LevelUp();
                    OverlayNextLevelUI.SetActive(true);
                    GameState = GameState.NextLevel;
                }
            }
            
            if (players[0].HasSignaledRefocus || players[1].HasSignaledRefocus || players[2].HasSignaledRefocus)
            {
                GameState = GameState.Syncing;
                int requester = players[0].HasSignaledRefocus ? 0 : (players[1].HasSignaledRefocus ? 1 : 2);
                _thalamusConnector.RefocusRequest(requester);
            }

            if ((players[0].HasSignaledStar || players[1].HasSignaledStar || players[2].HasSignaledStar) && Stars > 0)
            {
                GameState = GameState.UseStar;
                int requester = players[0].HasSignaledStar ? 0 : (players[1].HasSignaledStar ? 1 : 2);
                _thalamusConnector.StarRequest(requester);

            }
        }

        if (GameState == GameState.NextLevel)
        {
            if (players[0].IsReadyForNextLevel && players[1].IsReadyForNextLevel && players[2].IsReadyForNextLevel)
            {
                NextLevel();
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].IsReadyForNextLevel = false;
                }
            }
        }

        if (GameState == GameState.Mistake)
        {
            if (players[0].IsReady() && players[1].IsReady() && players[2].IsReady())
            {
                if (Lives == 0)
                {
                    GameFinishedTextUI.GetComponent<Text>().text = "Game Over";
                    GameFinishedTextUI.SetActive(true);
                    GameState = GameState.GameFinished;
                    points += Level;
                    ngames++;
                    _thalamusConnector.GameOver(Level);
                }
                else
                {
                    ContinueAfterMistake();
                    for (int i = 0; i < players.Length; i++)
                    {
                        players[i].IsReadyToContinue = false;
                    }
                }
            }
        }
       
        if (GameState == GameState.UseStar)
        {
            OverlayStarUI.SetActive(true);
            if (players[0].AgreeStar != null && players[1].AgreeStar != null && players[2].AgreeStar != null)
            {
                if (players[0].AgreeStar == "YES" && players[1].AgreeStar == "YES" && players[2].AgreeStar == "YES")
                {
                    foreach (Player p in players)
                    {
                        p.HasSignaledStar = false;
                        p.AgreeStar = null;
                        p.UseStarUpdate();
                    }
                    _thalamusConnector.AllAgreeStar();
                    
                    Stars--;
                    StarsUI.GetComponent<Text>().color = new Color(206, 17, 38);
                    UpdateStarsUI();
                    InvokeRepeating("ShrinkUntilDeactiveStarUI", 0, 0.006f);
                    GameState = GameState.Syncing;

                }
                else
                {
                    _thalamusConnector.NotAllAgreeStar();
                    foreach (Player p in players)
                    {
                        p.HasSignaledStar = false;
                        p.AgreeStar = null;
                    }
                    InvokeRepeating("ShrinkUntilDeactiveStarUI", 0, 0.006f);
                    GameState = GameState.Syncing;
                }
            }
            
           
            
        }

    }

  

    void UpdateNumLevelsSetupUI()
    {
        if (GameState == GameState.Connection)
        {
            MaxLevelsInputFieldUI.SetActive(true);
            MaxLevelsInputFieldUI.GetComponentInChildren<Button>().interactable = false;
        }
        else if (GameState == GameState.GameFinished)
        {
            MaxLevelsInputFieldUI.SetActive(true);
            MaxLevelsInputFieldUI.GetComponentInChildren<Button>().interactable = true;
        }
        else
        {
            MaxLevelsInputFieldUI.SetActive(false);
        }
    }

    void ValidateMove()
    {

        bool mistake = false;
        List<List<int>> wrongCards = new List<List<int>>();

        foreach (Player p in players)
        {
            List<int> playerWrongCards = p.GetWrongCards(topOfThePile);
            
            mistake = playerWrongCards.Count > 0 || mistake;

            if (playerWrongCards.Count == 0)
            {
                playerWrongCards.Add(0);
            }

            wrongCards.Add(playerWrongCards);
        }

        if (mistake)
        {
            _thalamusConnector.Mistake(pile.LastPlayer, topOfThePile, wrongCards[0].ToArray(), wrongCards[1].ToArray(), wrongCards[2].ToArray());
            audioSource.PlayOneShot(errorSound);
            Lives--;
            LivesUI.GetComponent<Text>().color = new Color(206, 17, 38);
            UpdateLivesUI();
            OverlayMistakeUI.SetActive(true);
            GameState = GameState.Mistake;
        }
        else
        {
            _thalamusConnector.CardPlayed(pile.LastPlayer, topOfThePile);
            audioSource.PlayOneShot(playingCardSound);
        }
    }

    private int HowManyPlayersLeft()
    {
        int countFinishedPlayers = 0;
        foreach (Player p in players)
        {
            if (p.HowManyCardsLeft() == 0)
            {
                countFinishedPlayers++;
            }
        }
        return NumPlayers - countFinishedPlayers;
    }

    private void ContinueAfterMistake()
    {
        if (HowManyPlayersLeft() > 1)
        {
            GameState = GameState.Syncing;
            _thalamusConnector.RefocusRequest(4);
        }
        else
        {
            GameState = GameState.Game;
            if (HowManyPlayersLeft() == 1)
            {
                _thalamusConnector.RefocusRequest(-1);
            }
        }
        LivesUI.GetComponent<Text>().color = new Color(238,238,238);
        UpdateLivesUI();
        OverlayMistakeUI.SetActive(false);
    }

    private void NextLevel()
    {
        StartNewLevel();
        topOfThePile = pile.GetTopCard();
        LevelUI.GetComponent<Text>().color = new Color(238, 238, 238);
        StarsUI.GetComponent<Text>().color = new Color(229, 234, 255);
        LivesUI.GetComponent<Text>().color = new Color(229, 234, 255);
        OverlayNextLevelUI.SetActive(false);
    }

    void ShrinkUntilDeactiveSyncingUI()
    {
        Vector3 scaleChange = new Vector3(-0.01f, -0.01f, 0.00f);
        OverlaySyncingUI.transform.localScale += scaleChange;
        if (OverlaySyncingUI.transform.localScale.x <= 0.02 || OverlaySyncingUI.transform.localScale.y <= 0.02)
        {
            OverlaySyncingUI.SetActive(false);
            OverlaySyncingUI.transform.localScale = new Vector3(1.2f, 1.0f, 0.00f);
            CancelInvoke();
        }
    }

    void ShrinkUntilDeactiveStarUI()
    {
        Vector3 scaleChange = new Vector3(-0.01f, -0.01f, 0.00f);
        OverlayStarUI.transform.localScale += scaleChange;
        if (OverlayStarUI.transform.localScale.x <= 0.02 || OverlayStarUI.transform.localScale.y <= 0.02)
        {
            OverlayStarUI.SetActive(false);
            OverlayStarUI.transform.localScale = new Vector3(1.2f, 1.0f, 0.00f);
            CancelInvoke();
        }
        StarsUI.GetComponent<Text>().color = new Color(229, 234, 255);
    }

    public static List<T> Randomize<T>(List<T> list)
    {
        List<T> randomizedList = new List<T>();
        //Random rnd = new Random();
        while (list.Count > 0)
        {
            int index = Random.Range(0, list.Count); //pick a random item from the master list
            randomizedList.Add(list[index]); //place it at the end of the randomized list
            list.RemoveAt(index);
        }
        return randomizedList;
    }
    List<List<int>> DealCards()
    {
        UnityEngine.Debug.Log("poits: " + points);
        UnityEngine.Debug.Log("ngames" + ngames);
        List<List<int>> hands = new List<List<int>>();
        List<int> cards = new List<int>();

        if (((points == 10 && ngames == 1) || (ngames > 2)) && ngames != 0)
        {
            UnityEngine.Debug.Log("Random");
            while (cards.Count < NumPlayers * Level)
            {
                int nextCard = Random.Range(1, 100);
                if (!cards.Contains(nextCard))
                {
                    cards.Add(nextCard);
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("Manipulado");
            if (Level == 1)
            {
                cards.Add(Random.Range(1, 20));
                cards.Add(Random.Range(50, 60));
                cards.Add(Random.Range(90, 100));
            }

            if (Level == 2)
            {
                while (cards.Count < 5)
                {
                    int nextCard = Random.Range(50, 100);
                    if (!cards.Contains(nextCard))
                    {
                        cards.Add(nextCard);
                    }
                }
                bool b = true;
                while (b)
                {
                    int c = cards[Random.Range(2, 3)] + 2;
                    if (!cards.Contains(c))
                    {
                        cards.Add(c);
                        b = false;
                    }
                }
            }

            if (Level == 3)
            {
                while (cards.Count < 7)
                {
                    int nextCard = Random.Range(1, 100);
                    if (!cards.Contains(nextCard))
                    {
                        cards.Add(nextCard);
                    }
                }
                bool b = true;
                while (b)
                {
                    int c = cards[Random.Range(0, 2)] + 3;
                    if (!cards.Contains(c))
                    {
                        cards.Insert(5, c);
                        b = false;
                    }
                }
                b = true;
                while (b)
                {
                    int c = cards[Random.Range(0, 5)] + 1;
                    if (!cards.Contains(c))
                    {
                        cards.Add(c);
                        b = false;
                    }
                }


            }

            if (Level == 4)
            {
                cards.Add(1);
                int e = 0;
                int f = 0;
                int g = 0;

                while (cards.Count < 11)
                {
                    if (e < 3)
                    {
                        int nextCard = Random.Range(1, 20);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            e++;
                        }
                    }
                    if (f < 3)
                    {
                        int nextCard = Random.Range(50, 70);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            f++;
                        }
                    }
                    if (g < 4)
                    {
                        int nextCard = Random.Range(75, 100);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            g++;
                        }
                    }
                }

                cards = Randomize(cards);

                bool b = true;
                while (b)
                {
                    int c = cards[Random.Range(8, 10)] + 2;
                    if (!cards.Contains(c))
                    {
                        cards.Add(c);
                        b = false;
                    }
                }

            }

            if (Level == 5)
            {
                int f = 0;
                int g = 0;

                while (cards.Count < 13)
                {
                    if (f < 7)
                    {
                        int nextCard = Random.Range(1, 20);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            f++;
                        }
                    }
                    if (g < 7)
                    {
                        int nextCard = Random.Range(70, 100);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            g++;
                        }
                    }
                }

                cards = Randomize(cards);

                bool b = true;
                while (b)
                {
                    int c = cards[Random.Range(5, 8)] + 2;
                    if (!cards.Contains(c))
                    {
                        cards.Insert(9, c);
                        b = false;
                    }
                }
                b = true;
                while (b)
                {
                    int c = cards[Random.Range(0, 9)] + 5;
                    if (!cards.Contains(c))
                    {
                        cards.Add(c);
                        b = false;
                    }
                }

            }

            if (Level == 6)
            {
                int f = 0;
                int g = 0;

                while (cards.Count < 18)
                {
                    if (f < 9)
                    {
                        int nextCard = Random.Range(1, 20);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            f++;
                        }
                    }
                    if (g < 9)
                    {
                        int nextCard = Random.Range(70, 100);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            g++;
                        }
                    }
                }

                cards = Randomize(cards);
            }

            if (Level == 7)
            {
                int f = 0;
                int g = 0;

                while (cards.Count < 19)
                {
                    if (f < 3)
                    {
                        int nextCard = Random.Range(1, 10);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            f++;
                        }
                    }
                    if (g < 16)
                    {
                        int nextCard = Random.Range(40, 100);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            g++;
                        }
                    }
                }

                cards = Randomize(cards);

                bool b = true;
                while (b)
                {
                    int c = cards[Random.Range(0, 5)] + 2;
                    if (!cards.Contains(c))
                    {
                        cards.Insert(6, c);
                        b = false;
                    }
                }

                b = true;
                while (b)
                {
                    int c = cards[Random.Range(7, 13)] + 1;
                    if (!cards.Contains(c))
                    {
                        cards.Add(c);
                        b = false;
                    }
                }

            }

            if (Level == 8)
            {
                int e = 0;
                int f = 0;
                int g = 0;

                while (cards.Count < 24)
                {
                    if (e < 8)
                    {
                        int nextCard = Random.Range(1, 20);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            e++;
                        }
                    }
                    if (f < 8)
                    {
                        int nextCard = Random.Range(25, 40);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            f++;
                        }
                    }
                    if (g < 8)
                    {
                        int nextCard = Random.Range(50, 100);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            g++;
                        }
                    }
                }

                cards = Randomize(cards);
            }

            if (Level == 9)
            {
                int f = 0;
                int g = 0;

                while (cards.Count < 25)
                {
                    if (f < 9)
                    {
                        int nextCard = Random.Range(1, 25);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            f++;
                        }
                    }
                    if (g < 16)
                    {
                        int nextCard = Random.Range(50, 100);
                        if (!cards.Contains(nextCard))
                        {
                            cards.Add(nextCard);
                            g++;
                        }
                    }
                }

                cards = Randomize(cards);

                bool b = true;
                while (b)
                {
                    int c = cards[Random.Range(9, 16)] + 3;
                    if (!cards.Contains(c))
                    {
                        cards.Insert(17, c);
                        b = false;
                    }
                }

                b = true;
                while (b)
                {
                    int c = cards[Random.Range(0, 8)] + 1;
                    if (!cards.Contains(c))
                    {
                        cards.Add(c);
                        b = false;
                    }
                }
            }

            if (Level == 10)
            {
                while (cards.Count < NumPlayers * Level)
                {
                    int nextCard = Random.Range(1, 100);
                    if (!cards.Contains(nextCard))
                    {
                        cards.Add(nextCard);
                    }
                }
            }
        }

        for (int i = 0; i < NumPlayers; i++)
        {
            List<int> hand = cards.GetRange(i * Level, Level);
            hands.Add(hand);
        }
        return hands;
    }

    void LevelUp()
    {
        if (Level == 2 || Level == 5 || Level == 8)
        {
            Stars++;
            StarsUI.GetComponent<Text>().text = "Stars: " + Stars;
            StarsUI.GetComponent<Text>().color = new Color(122, 184, 0);
        }
        if (Level == 3 || Level == 6 || Level == 9)
        {
            Lives++;
            LivesUI.GetComponent<Text>().text = "Lives: " + Lives;
            LivesUI.GetComponent<Text>().color = new Color(122, 184, 0);

        }
        Level++;
        LevelUI.GetComponent<Text>().text = "Level: " + Level;
        LevelUI.GetComponent<Text>().color = new Color(122, 184, 0);

    }

    void UpdateLivesUI()
    {
        LivesUI.GetComponent<Text>().text = "Lives: " + Lives;
    }

    void UpdateStarsUI()
    {
        StarsUI.GetComponent<Text>().text = "Stars: " + Stars;
    }

    void StartNewLevel()
    {
        List<List<int>> hands = DealCards();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].ReceiveCards(hands[i]);
            players[i].showCards.Clear();
        }
        pile.StartNewLevel();
        _thalamusConnector.StartLevel(Level, Stars, Lives, hands[0].ToArray(), hands[1].ToArray(), hands[2].ToArray());
        GameState = GameState.Syncing;
    }

    public void ChangeThalamusClientIP()
    {
        IPadress = IPinputField.GetComponent<InputField>().text;
    }

    public void ChangeMaxLevel()
    {
        int max = int.Parse(MaxLevelsInputFieldUI.GetComponent<InputField>().text);
        MaxLevels = max;
    }

    public void ChangeDebugMode()
    {
        DebugMode = MaxLevelsInputFieldUI.GetComponentInChildren<Toggle>().isOn;
    }

    public void StartFromLevelOne()
    {
        Level = 1;
        Lives = 3;
        Stars = 1;
        OverlayMistakeUI.SetActive(false);
        GameFinishedTextUI.SetActive(false);
        _thalamusConnector.AllConnected(MaxLevels, players[0].ID, players[0].Name, players[1].ID, players[1].Name, players[2].ID, players[2].Name);
        OverlayNextLevelUI.SetActive(true);
        UpdateLivesUI();
        UpdateStarsUI();
        GameState = GameState.NextLevel;
    }

}
