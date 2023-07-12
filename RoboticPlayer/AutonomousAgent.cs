using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thalamus;
using TheMindThalamusMessages;
using GazeOFMessages;
using EmoteCommonMessages;
using Thalamus.BML;

namespace RoboticPlayer
{
    public enum GameState
    {
        Connection,
        Syncing,
        Game,
        Mistake,
        NextLevel,
        GameFinished,
        NoMoreCards,
        Waiting,
        StopMainLoop,
        Wait,
        UseStar
    }

   
    public interface IGazeBehaviours : IPerception
    {
        void GazeBehaviourStarted(string gazer, string target, int timestamp);
        void GazeBehaviourFinished(string gazer, string target, int timestamp);

    }

    public interface IAutonomousAgentPublisher : IThalamusPublisher, ITabletsGM, IGazeStateActions, IGazeBehaviours, IAnimationActions, ISpeakActions, ISpeakControlActions, IPostureActions { }

    class AutonomousAgent : ThalamusClient, IGMTablets, IGazeOpenFacePerceptions
    {
        public class TheMindPublisher : IAutonomousAgentPublisher
        {
            dynamic publisher;
            public TheMindPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void ConnectToGM(int playerID, string name)
            {
                this.publisher.ConnectToGM(playerID, name);
            }

            public void ContinueAfterMistake(int playerID)
            {
                this.publisher.ContinueAfterMistake(playerID);
            }

            public void PlayCard(int playerID, int card)
            {
                this.publisher.PlayCard(playerID, card);
            }

            public void StarSignal(int playerID)
            {
                this.publisher.StarSignal(playerID);
            }
            public void NoStarSignal(int playerID)
            {
                this.publisher.NoStarSignal(playerID);
            }
            public void YesStarSignal(int playerID)
            {
                this.publisher.YesStarSignal(playerID);
            }
            public void ReadyForNextLevel(int playerID)
            {
                this.publisher.ReadyForNextLevel(playerID);
            }

            public void RefocusSignal(int playerID)
            {
                this.publisher.RefocusSignal(playerID);
            }

            public void GazeAtScreen(double x, double y)
            {
                publisher.GazeAtScreen(x, y);
            }

            public void GazeAtTarget(string targetName)
            {
                publisher.GazeAtTarget(targetName);
            }

            public void GlanceAtScreen(double x, double y)
            {
                publisher.GlanceAtScreen(x, y);
            }

            public void GlanceAtTarget(string targetName)
            {
                publisher.GlanceAtTarget(targetName);
            }

            public void GazeBehaviourStarted(string gazer, string target, int timestamp)
            {
                publisher.GazeBehaviourStarted(gazer, target, timestamp);
            }

            public void GazeBehaviourFinished(string gazer, string target, int timestamp)
            {
                publisher.GazeBehaviourFinished(gazer, target, timestamp);
            }

            
            public void PlayAnimation(string playerID, string animation)
            {
                publisher.PlayAnimation(playerID, animation);
            }

            public void PlayAnimationQueued(string playerID, string animation)
            {
                publisher.PlayAnimationQueued(playerID, animation);
            }

            public void StopAnimation(string playerID)
            {
                publisher.StopAnimation(playerID);
            }

            public void Speak(string playerID, string text)
            {
                publisher.Speak(playerID, text);
            }

            public void SpeakBookmarks(string playerID, string[] text, string[] bookmarks)
            {
                publisher.SpeakBookmarks(playerID, text, bookmarks);
            }
            public void SpeakStop()
            {
                publisher.SpeakStop();
            }
            public void SetLanguage(SpeechLanguages language)
            {
                publisher.SetLanguage(language);
            }
            public void SetOutputDevice(int device)
            {
                publisher.SetOuyputDevice(device);
            }
            public void SetPosture(string playerID, string posture, double percent, double decay)
            {
                publisher.SetPosture(playerID, posture, percent, decay);
            }
            public void ResetPose()
            {
                publisher.ResetPose();
            }

        }

        public TheMindPublisher TMPublisher;
        public GazeController gazeController;
        protected int ID;
        public GameState _gameState;
        protected GameState _previousGameState;
        protected List<GameState> eventsList;
        protected static Mutex mut = new Mutex();
        protected Random randomNums;
        protected int MaxLevel;
        protected int TopOfThePile;
        protected float Pace;
        protected List<int> cards;
        protected List<int> cardsLeft;
        protected Stopwatch PlayStopWatch;
        protected Stopwatch lastCardStopWatch;
        public Stopwatch SessionStartStopWatch;
        protected int nextTimeToPlay;
        private string GazeType;
        public bool lookattablet;
        public int lookatplayer;
        public bool lookatfront;
        public bool lookrandom;
        public int LEVEL;
        private static Thread tabletThread;
        private static Thread playerThread;
        private static Thread lookatfrontThread;
        private static Thread lookrandomThread;
        public int rlook;

        public AutonomousAgent(string clientName, string character, int playerID, string gazeType)
            : base(clientName, character)
        {

            SetPublisher<IAutonomousAgentPublisher>();
            TMPublisher = new TheMindPublisher(base.Publisher);
            GazeType = gazeType;
            
            if (gazeType == "r")
            {
                gazeController = new RandomGazeController(this);
                //gazeController.JOINT_ATTENTION = false;
            }
            else if (gazeType == "m")
            {
                gazeController = new ReactiveGazeController(this);
                //gazeController.JOINT_ATTENTION = false;
            }
            else if (gazeType == "p")
            {
                gazeController = new ProactiveGazeController(this);
                //gazeController.JOINT_ATTENTION = true;
            }

            Thread gazeThread = new Thread(gazeController.Update);
            gazeThread.Start();

            ID = playerID;
            TopOfThePile = 0;
            Pace = 1000;
            _gameState = GameState.Waiting;
            eventsList = new List<GameState>();
            randomNums = new Random();
            PlayStopWatch = new Stopwatch();
            lastCardStopWatch = new Stopwatch();
            SessionStartStopWatch = new Stopwatch();
            SessionStartStopWatch.Start();
            nextTimeToPlay = -1;
            lookattablet = false;
            lookatplayer = -1;
            lookrandom = false;
            lookatfront = false;
            rlook = 10000;

            TMPublisher.SetLanguage(SpeechLanguages.Portuguese);
            //Thread mainLoopThread = new Thread(MainLoop);
            //mainLoopThread.Start();


        }
        public void MainLoop()
        {
            while(_gameState != GameState.StopMainLoop)
            {
                
                /*mut.WaitOne();
                if (_gameState == GameState.Waiting && eventsList.Count > 0)
                {
                    //Console.WriteLine(String.Join(", ", eventsList));
                    //_gameState = eventsList[0];
                    //eventsList.RemoveAt(0);
                    Console.WriteLine(_gameState);
                }
                mut.ReleaseMutex();*/
                
                if (_gameState == GameState.Wait)
                {
                    //do nothing
                }
                if (_gameState == GameState.UseStar)
                {
                    
                    lookatfront = true;
                    LookAtFront(3000);
                    List<string> texto = new List<string> { "Uma estrela!", "Boa ideia!", "ok!" };
                    Random random = new Random();
                    int randomIndex = random.Next(texto.Count);
                    TMPublisher.SetPosture("player2", "neural", 0, 0);
                    TMPublisher.Speak("player2", texto[randomIndex]);

                    int randomWait = randomNums.Next(2000, 5000);
                    Thread.Sleep(randomWait);
                    lookattablet = true;
                    LookAtTablet();

                    //mut.WaitOne();
                    TMPublisher.YesStarSignal(ID);
                    nextTimeToPlay = -1;
                    _gameState = GameState.Waiting;
                    //mut.ReleaseMutex();
                }
                if (_gameState == GameState.NextLevel)
                {
                    //mut.WaitOne();
                    if (LEVEL >= 1) //(cardsLeft[0] == 0 && cardsLeft[1] == 0)
                    {
                        //Console.WriteLine("[{0}]", string.Join(", ", cardsLeft));
                        while (true) 
                        { 
                            Console.WriteLine("[{0}]", string.Join(", ", cardsLeft));
                            if (cardsLeft[0] == 0 & cardsLeft[1] == 0)
                            {
                                lookatfront = true;
                                LookAtFront(3000);
                                List<string> texto = new List<string> { "Boa! Passamos um nivel!", "Mais um nivel!" };
                                Random random = new Random();
                                int randomIndex = random.Next(texto.Count);
                                TMPublisher.PlayAnimation("player2", "joy1");
                                TMPublisher.Speak("player2", texto[randomIndex]);
                                lookatfront = true;
                                LookAtFront(3000);
                                break;
                            }
                            //await Task.Delay(1);
                        }
                    }
                    int randomWait = randomNums.Next(2000, 5000);
                    Thread.Sleep(randomWait);
                    lookattablet = true;
                    LookAtTablet();
                    TMPublisher.ReadyForNextLevel(ID);
                    _gameState = GameState.Waiting;
                    //mut.ReleaseMutex();
                }
                if (_gameState == GameState.Syncing)
                {
                    int randomWait = randomNums.Next(2000, 5000);
                    Thread.Sleep(randomWait);
                    lookattablet = true;
                    LookAtTablet();
                    
                    //mut.WaitOne();
                    TMPublisher.RefocusSignal(ID);
                    _gameState = GameState.Waiting;
                    //mut.ReleaseMutex();
                }
                if (_gameState == GameState.Mistake)
                {
                    lookatfront = true;
                    LookAtFront(1000);
                    List<string> triste = new List<string> { "oh não!", "Perdemos uma vida!"};
                    Random random = new Random();
                    int randomIndex = random.Next(triste.Count);
                    TMPublisher.SetPosture("player2", "disappointment", 0, 0);
                    TMPublisher.Speak("player2", triste[randomIndex]);
                    
                    int randomWait = randomNums.Next(2000, 5000);
                    Thread.Sleep(randomWait);
                    lookattablet = true;
                    LookAtTablet();
                    
                    //mut.WaitOne();
                    TMPublisher.ContinueAfterMistake(ID);
                    nextTimeToPlay = -1;
                    _gameState = GameState.Waiting;
                    //mut.ReleaseMutex();
                }
                if (_gameState == GameState.Game)
                    {
                        mut.WaitOne();
                        if (nextTimeToPlay == -1)
                        {
                            if (cards.Count > 0)
                            {
                                if (ID == 2 && cardsLeft[0] == 0 && cardsLeft[1] == 0)
                                {
                                    PlayStopWatch.Restart();
                                    nextTimeToPlay = 1500;
                                }
                                else
                                {
                                    PlayStopWatch.Restart();
                                    nextTimeToPlay = EstimateTimeToPlay();
                                    //Console.WriteLine(">>>>> NextTimeToPlay in " + (lowestCard - TopOfThePile) + "s : " + lowestCard + " - " + TopOfThePile + " / " + cardsLeft[0] + " / " + cardsLeft[1] + " / " + cardsLeft[2]);
                                    Console.WriteLine(">>>>> NextTimeToPlay in " + nextTimeToPlay);
                                }
                            }
                            else
                            {
                                //_gameState = GameState.Waiting;
                                //Console.WriteLine("---- No more cards!!!!!");
                            }
                        }
                        if (nextTimeToPlay >= PlayStopWatch.ElapsedMilliseconds + 10000)
                        {
                            if (PlayStopWatch.ElapsedMilliseconds >= rlook && PlayStopWatch.ElapsedMilliseconds <= rlook+1000)
                            {
                                rlook += 10000;
                                Console.WriteLine(rlook);
                                int r = randomNums.Next(0,2);
                                if (r == 0)
                                {
                                    lookrandom = true;
                                    LookRandom();
                                }
                                if (r == 1)
                                {
                                    lookattablet = true;
                                    LookAtTablet();
                                }
                            }
                        }
                        else if (PlayStopWatch.IsRunning && PlayStopWatch.ElapsedMilliseconds >= (nextTimeToPlay - 4000) && PlayStopWatch.ElapsedMilliseconds <= nextTimeToPlay)
                        {
                            lookattablet = true;
                            LookAtTablet();
                        }
                        else if (PlayStopWatch.IsRunning && PlayStopWatch.ElapsedMilliseconds >= nextTimeToPlay)
                        {
                            PlayStopWatch.Stop();
                            TMPublisher.PlayCard(ID, cards[0]);
                            cards.RemoveAt(0);
                            nextTimeToPlay = -1;
                    }
                        mut.ReleaseMutex();
                    }
                
                //Thread.Sleep(2000);
            }
        }

        public void LookAtTablet()
        {
            tabletThread = new Thread(() => 
            {
                Thread.Sleep(3000);
            });
            tabletThread.Start();
            tabletThread.Join();
            lookattablet = false;
           
            
        }
        public void LookAtPlayer()
        {
            playerThread = new Thread(() =>
            {
                Thread.Sleep(2000);
            });

            playerThread.Start();
            playerThread.Join();
            lookatplayer = -1;
            TMPublisher.SetPosture("player2", "neutral", 0, 0);
        }

        public void LookAtFront(int time)
        {
            lookatfrontThread = new Thread(() =>
            {
                Thread.Sleep(time);
            });

            lookatfrontThread.Start();
            lookatfrontThread.Join();
            lookatfront = false;
            

        }

        public void LookRandom()
        {
            lookrandomThread = new Thread(() =>
            {
                Thread.Sleep(3000);
            });

            lookrandomThread.Start();
            lookrandomThread.Join();
            lookrandom = false;


        }
        public virtual int EstimateTimeToPlay()
        {
            return (cards[0] - TopOfThePile) * 1000;
        }

        public void ConnectToGM()
        {
            TMPublisher.ConnectToGM(ID, "Agent-" + GazeType);
        }

        public void StopMainLoop()
        {
            //mut.WaitOne();
            _gameState = GameState.StopMainLoop;
            //mut.ReleaseMutex();
        }

        public override void Dispose()
        {
            //mut.WaitOne();
            _gameState = GameState.StopMainLoop;
            gazeController.Dispose();
            base.Dispose();
            //mut.WaitOne();
        }

        public void AllConnected(int maxLevel, int p0Id, string p0Name, int p1Id, string p1Name, int p2Id, string p2Name)
        {
            MaxLevel = maxLevel;
            mut.WaitOne();
            //eventsList.Add(GameState.NextLevel);
            _gameState = GameState.NextLevel;
            mut.ReleaseMutex();
            SessionStartStopWatch.Restart();
            
        }

        public void StartLevel(int level, int stars ,int teamLives, int[] p0Hand, int[] p1Hand, int[] p2Hand)
        {
            LEVEL = level;
            if (LEVEL == 1)
            {
                lookatfront = true;
                LookAtFront(3000);
                TMPublisher.Speak("player2", "Olá! Eu sou o António! E serei o vosso terceiro membro da equipa!");
                lookatfront = true;
                LookAtFront(10000);
            }
            TopOfThePile = 0;
            Pace = 1000;
            cards = new List<int>();
            cardsLeft = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                cardsLeft.Add(level);
            }

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
            else if (ID == 2)
            {
                foreach (int card in p2Hand)
                {
                    cards.Add(card);
                }
            }
            mut.WaitOne();
            //eventsList.Add(GameState.Syncing);
            _gameState = GameState.Syncing;
            mut.ReleaseMutex();
        }

        public void FinishLevel(int level, int teamLives)
        {
            if (level < MaxLevel)
            {
                mut.WaitOne();
                //eventsList.Add(GameState.NextLevel);
                _gameState = GameState.NextLevel;
                mut.ReleaseMutex();
            }
        }

        public void AllRefocused()
        {
            if (_gameState != GameState.NoMoreCards)
            {
                mut.WaitOne();
                //eventsList.Add(GameState.Game);
                _gameState = GameState.Game;
                lastCardStopWatch.Restart();
                mut.ReleaseMutex();

            }
        }

        public void RefocusRequest(int playerID)
        {
            
            if (playerID == -1)
            {
                mut.WaitOne();
                //eventsList.Add(GameState.Game);
                _gameState = GameState.Game;
                mut.ReleaseMutex();
            }
            else if (cards.Count > 0)
            {
                mut.WaitOne();
                //eventsList.Add(GameState.Syncing);
                _gameState = GameState.Syncing;
                mut.ReleaseMutex();
            }
            else
            {
                mut.WaitOne();
                //eventsList.Add(GameState.Waiting);
                _gameState = GameState.Waiting;
                mut.ReleaseMutex();
            }
            
        }

        public void CardPlayed(int playerID, int card)
        {
            Console.WriteLine(playerID);

            TMPublisher.SetPosture("player2", "satisfaction", 0, 0);

            if (playerID == 0 || playerID == 1)
            {
                if (cards.Count > 0)
                {
                    if (cards[0] == card + 1)
                    {
                        TMPublisher.Speak("player2", "Agora sou eu!");
                    }
                }
                else
                {
                    lookatplayer = playerID;
                    LookAtPlayer();
                }
            }

            rlook = 10000;

            mut.WaitOne();
            long timeDelta = lastCardStopWatch.ElapsedMilliseconds;
            int cardsDelta = card - TopOfThePile;
            Pace = timeDelta / cardsDelta;
            Console.WriteLine("New PACE: " + Pace);
            lastCardStopWatch.Restart();
            TopOfThePile = card;
            nextTimeToPlay = -1;
            cardsLeft[playerID]--;
            Console.WriteLine("[{0}]", string.Join("- ", cardsLeft));
            PlayStopWatch.Stop();
            mut.ReleaseMutex();
        }

        public void Mistake(int playerID, int card, int[] p0WrongCards, int[] p1WrongCards, int[] p2WrongCards)
        {
            Console.WriteLine("[{0}]", string.Join(", ", p0WrongCards));
            Console.WriteLine("[{0}]", string.Join(", ", p1WrongCards));
            Console.WriteLine("[{0}]", string.Join(", ", p2WrongCards));
            //mut.WaitOne();
            if (p0WrongCards == null)
            {
                p0WrongCards = new int[] { };
            }
            if (p1WrongCards == null)
            {
                p1WrongCards = new int[] { };
            }
            if (p2WrongCards == null)
            {
                p2WrongCards = new int[] { };
            }
            TopOfThePile = card;
            cardsLeft[playerID]--;

            bool shouldAckMistake = false;

            if (p0WrongCards.Sum() > 0)
            {
                cardsLeft[0] -= p0WrongCards.Length;
                shouldAckMistake = true;
            }

            if (p1WrongCards.Sum() > 0)
            {
                cardsLeft[1] -= p1WrongCards.Length;
                shouldAckMistake = true;
            }

            if (p2WrongCards.Sum() > 0)
            {
                cardsLeft[2] -= p2WrongCards.Length;
                foreach (int wrongCard in p2WrongCards)
                {
                    cards.Remove(wrongCard);
                    shouldAckMistake = true;
                }
            }

            /*if (playerID != ID)
            {
                if (ID == 0)
                {
                    Console.Write("id0");
                    if (p0WrongCards.Length > 0)
                    {
                        foreach (int wrongCard in p0WrongCards)
                        {
                            cards.Remove(wrongCard);
                            shouldAckMistake = true;
                        }
                    }
                    cardsLeft[1] -= p1WrongCards.Length;
                    cardsLeft[2] -= p2WrongCards.Length;
                }
                else if (ID == 1)
                {
                    Console.Write("id1");
                    if (p1WrongCards.Length > 0)
                    {
                        foreach (int wrongCard in p1WrongCards)
                        {
                            cards.Remove(wrongCard);
                            shouldAckMistake = true;
                        }
                    }
                    cardsLeft[0] -= p0WrongCards.Length;
                    cardsLeft[2] -= p2WrongCards.Length;
                }
                else if (ID == 2)
                {
                    Console.Write("id2");
                    if (p2WrongCards.Length > 0)
                    {
                        foreach (int wrongCard in p2WrongCards)
                        {
                            cards.Remove(wrongCard);
                            shouldAckMistake = true;
                        }
                    }
                    cardsLeft[0] -= p0WrongCards.Length;
                    cardsLeft[1] -= p1WrongCards.Length;
                }
            }
            else
            {
                cardsLeft[0] -= p0WrongCards.Length;
                cardsLeft[1] -= p1WrongCards.Length;
                cardsLeft[2] -= p2WrongCards.Length;
            }*/
            Console.WriteLine("[{0}]", string.Join(", ", cardsLeft));
            if (cards.Count > 0 || shouldAckMistake)
            {
                _gameState = GameState.Mistake;
            }
            //mut.ReleaseMutex();
        }
        public void GameOver(int level)
        {
            lookatfront = true;
            LookAtFront(3000);
            TMPublisher.PlayAnimation("player2", "sadness5");
            TMPublisher.Speak("player2", "oh Não! Perdemos o jogo!");
            //throw new NotImplementedException();
        }
        public void GameCompleted()
        {
            lookatfront = true;
            LookAtFront(3000);
            TMPublisher.PlayAnimation("player2", "joy5");
            TMPublisher.Speak("player2", " Ganhamos o jogo!");
            //throw new NotImplementedException();
        }
        public void StarRequest(int playerID)
        {
            mut.WaitOne();
            _gameState = GameState.UseStar;
            mut.ReleaseMutex();
        }
        public void AllAgreeStar()
        {
            mut.WaitOne();
            cards.RemoveAt(0);
            for (var i = 0; i < cardsLeft.Count; i++)
            {
                cardsLeft[i]--;
            }
            _gameState = GameState.Syncing;
            mut.ReleaseMutex();
            Console.WriteLine("[{0}]", string.Join("* ", cardsLeft));
        }
        public void NotAllAgreeStar()
        {
            mut.WaitOne();
            _gameState = GameState.Syncing;
            mut.ReleaseMutex();
        }
        public void StartWait()
        {
            mut.WaitOne();
            _previousGameState = _gameState;
            _gameState = GameState.Wait;
            mut.ReleaseMutex();
            
        }
        public void EndWait()
        {
            mut.WaitOne();
            if (cards.Count == 0)
            {
                _gameState = GameState.NextLevel;
            }
            else
            {
                _gameState = GameState.Game;
            }
            mut.ReleaseMutex();
        }
        public void GazeOpenFace(int faceId, double angleX, double angleY, string target, double timeMiliseconds)
        {
            if (faceId != ID && gazeController.SessionStarted)
            {
                if (gazeController.Player0.ID == faceId)
                {
                    gazeController.Player0.GazeEvent(target, SessionStartStopWatch.ElapsedMilliseconds);
                }
                else if (gazeController.Player1.ID == faceId)
                {
                    gazeController.Player1.GazeEvent(target, SessionStartStopWatch.ElapsedMilliseconds);
                }

            }
        }

        public void TargetCalibrationStarted(int faceId, string target)
        {
            //DO SOMETHING
        }

        public void TargetCalibrationFinished(int faceId, string target)
        {
            //DO SOMETHING
        }

        public void CalibrationPhaseFinished(int faceId)
        {
            if (faceId == gazeController.Player0.ID)
            {
                gazeController.Player0.SessionStarted = true;
            }
            else if (faceId == gazeController.Player1.ID)
            {
                gazeController.Player1.SessionStarted = true;
            }

            if (gazeController.Player0.SessionStarted && gazeController.Player1.SessionStarted)
            {
                gazeController.SessionStarted = true;
            }
        }


        
    }
}
