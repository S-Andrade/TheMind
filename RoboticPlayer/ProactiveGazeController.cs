using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheMindThalamusMessages;

namespace RoboticPlayer
{
    class ProactiveGazeController : GazeController
    {
        private Random random;
        private long nextgazeShift;
        private string[] targets;
        private string PLAYER_A;
        private string PLAYER_B;
        private string SCREEN;
        private string TABLET;
        private string FRONT;
        private string RANDOM1;
        private string RANDOM2;
        private string RANDOM3;
        private string RANDOM4;
        private string RANDOM5;
        private string target;
        private int AVG_DUR_SCREEN;
        private int AVG_DUR_PLAYER;
        private Stopwatch LookStopWatch;
        private int nextTimeToLook;
        private int gazeTime;
        private string nextTarget;
        private int lookp1;
        private int lookp0;
        private Stopwatch WriteDuration;

        public ProactiveGazeController(AutonomousAgent thalamusClient) : base(thalamusClient)
        {
            random = new Random();
            PLAYER_A = "player0";
            PLAYER_B = "player1";
            SCREEN = "mainscreen";
            TABLET = "tablet";
            FRONT = "front";
            RANDOM1 = "random1";
            RANDOM2 = "random2";
            RANDOM3 = "random3";
            RANDOM4 = "random4";
            RANDOM5 = "random5";
            AVG_DUR_SCREEN = 5000;
            AVG_DUR_PLAYER = 2000;
            targets = new string[] { PLAYER_A, PLAYER_B, SCREEN, TABLET };
            target = SCREEN;
            LookStopWatch = new Stopwatch();
            nextTimeToLook = -1;
            gazeTime = 1000;
            nextTarget = PLAYER_A;
            lookp0 = 0;
            lookp1 = 0;
        }


        public override void Update()
        {
            int q = 0;
            Console.WriteLine("Proactive");
            while (true)
            {
                if (SessionStarted)
                {
                    
                    if (currentGazeDuration.ElapsedMilliseconds >= 1000)
                    {
                        string target = "";

                        if (aa.lookatplayer != -1)
                        {
                            if (aa.lookatplayer == 0)
                            {
                                target = PLAYER_A;
                                lookp0 += 1;
                            }
                            if (aa.lookatplayer == 1)
                            {
                                target = PLAYER_B;
                                lookp1 += 1;
                            }
                        }
                        else if (aa.lookattablet)
                        {
                            target = TABLET;
                        }
                        else if (aa.lookatfront)
                        {
                            if (dois == 0)
                            {
                                Console.WriteLine("zero");
                                List<string> r = new List<string> { PLAYER_A, PLAYER_B };
                                target = r[random.Next(r.Count)];
                                lastlook = target;
                                dois = 2;
                            }
                            else if (dois == 1)
                            {
                                Console.WriteLine("um");
                                if (lastlook == PLAYER_A)
                                {
                                    Console.WriteLine("aiaiaiaiai");
                                    target = PLAYER_B;
                                    lastlook = target;
                                }
                                else if (lastlook == PLAYER_B)
                                {
                                    Console.WriteLine("uiuiuiuiuiu");
                                    target = PLAYER_A;
                                    lastlook = target;
                                }
                                dois = 2;
                            }
                            else if (dois < 3)
                            {
                                Console.WriteLine("n");
                                Console.Write(dois);
                                target = lastlook;
                                dois++;
                            }
                            else if (dois == 3)
                            {
                                Console.WriteLine("tres");
                                target = lastlook;
                                dois = 1;
                            }
                            Console.WriteLine(lastlook);
                            Console.WriteLine(target);
                        }
                        else if (aa._gameState == GameState.Game || aa._gameState == GameState.Waiting || aa._gameState == GameState.NextLevel)
                        {
                            if (nextTimeToLook == -1)
                            {
                                LookStopWatch.Restart();
                                nextTimeToLook = random.Next(3000, 5000);
                                gazeTime = random.Next(500, 3000);
                                Console.WriteLine("nextTimeToLook " + nextTimeToLook);
                                Console.WriteLine("gazeTime " + gazeTime);
                                

                                if (lookp0 < lookp1) { nextTarget = PLAYER_A; lookp0 += 1; }
                                else if (lookp1 < lookp0) { nextTarget = PLAYER_B; lookp1 += 1; }
                                else {
                                    List<string> r = new List<string> { PLAYER_A, PLAYER_B };
                                    nextTarget = r[random.Next(r.Count)];
                                }


                            }
                            if (LookStopWatch.IsRunning && LookStopWatch.ElapsedMilliseconds >= nextTimeToLook)
                            {   
                                target = nextTarget;
                                Console.WriteLine(target);

                            }
                            if (LookStopWatch.IsRunning && LookStopWatch.ElapsedMilliseconds >= gazeTime + nextTimeToLook)
                            {
                                target = SCREEN;
                                LookStopWatch.Stop();
                                nextTimeToLook = -1;
                                Console.WriteLine("end");
                            }
                        }
                        else
                        {
                            target = SCREEN;
                        }


                        //if (currentTarget == TABLET)
                        //{
                        //aa.TMPublisher.SetPosture("player2", "neutral", 0, 0);
                        //}

                        aa.TMPublisher.GazeBehaviourFinished("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);
                        currentTarget = target;
                        //Console.WriteLine("------------------------ gaze at " + currentTarget);
                        aa.TMPublisher.GazeAtTarget(currentTarget);
                        currentGazeDuration.Restart();
                        aa.TMPublisher.GazeBehaviourStarted("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);

                        using (StreamWriter sw = File.AppendText("C:\\Users\\sandr\\Desktop\\the-mind-main\\timestampProactive.txt"))
                        {
                            sw.WriteLine(DateTime.Now + " P0 " + Player0.CurrentGazeBehaviour.Target + " P1 " + Player1.CurrentGazeBehaviour.Target + " P2 " + currentTarget);
                        }
                    }

                }
            }
        }
    }
}
