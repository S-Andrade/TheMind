using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private string RANDOM;
        private string target;
        private int AVG_DUR_SCREEN;
        private int AVG_DUR_PLAYER;
        private Stopwatch LookStopWatch;
        private int nextTimeToLook;
        private int times;
        private int timesp0;
        private int timesp1;

        public ProactiveGazeController(AutonomousAgent thalamusClient) : base(thalamusClient)
        {
            random = new Random();
            PLAYER_A = "player0";
            PLAYER_B = "player1";
            SCREEN = "mainscreen";
            TABLET = "tablet";
            FRONT = "front";
            RANDOM = "random";
            AVG_DUR_SCREEN = 5000;
            AVG_DUR_PLAYER = 2000;
            targets = new string[] { PLAYER_A, PLAYER_B, SCREEN, TABLET };
            target = SCREEN;
            LookStopWatch = new Stopwatch();
            nextTimeToLook = -1;
            times = 0;
            timesp0 = 0;
            timesp1 = 0;

        }

        public override void Update()
        {
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
                                times++;
                            }
                            if (aa.lookatplayer == 1)
                            {
                                target = PLAYER_B;
                                times++;
                            }
                        }
                        else if (aa.lookattablet)
                        {
                            target = TABLET;
                        }
                        else if (aa.lookatfront)
                        {
                            target = FRONT;
                        }

                        else if (aa._gameState == GameState.Game)
                        {
                            if (nextTimeToLook == -1)
                            {
                                LookStopWatch.Restart();
                                nextTimeToLook = random.Next(4000, 6000);
                            }
                            if (LookStopWatch.IsRunning && LookStopWatch.ElapsedMilliseconds >= nextTimeToLook)
                            {
                                LookStopWatch.Stop();
                                nextTimeToLook = -1;
                                
                                if (times == 0 || timesp0 == timesp1)
                                {
                                    List<string> list = new List<string> {PLAYER_A,PLAYER_B};
                                    target = list[random.Next(2)];
                                }
                                else if (timesp0/times > 0.70)
                                {
                                    target = PLAYER_A;
                                }
                                else if (timesp1 / times > 0.70)
                                {
                                    target = PLAYER_B;
                                }
                                else
                                {
                                    List<string> list = new List<string> { PLAYER_A, PLAYER_B };
                                    target = list[random.Next(2)];
                                }



                                times++;


                            }
                        }

                        else if (aa.lookrandom)
                        {
                            target = RANDOM;
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
                        if (currentTarget == PLAYER_A)
                        {
                            timesp0++;
                        }
                        if (currentTarget == PLAYER_B)
                        {
                            timesp1++;
                        }
                        Console.WriteLine(currentTarget);

                        using (StreamWriter writer = new StreamWriter("C:\\Users\\sandr\\Desktop\\the-mind-main\\proactive.txt"))
                        {
                            if ((Player0.IsGazingAtRobot() && currentTarget == "player0") || (Player1.IsGazingAtRobot() && currentTarget == "player1"))
                            {
                                MutualGaze++;
                                writer.WriteLine("MG " + MutualGaze);
                            }
                            if (Player0.CurrentGazeBehaviour.Target == Player1.CurrentGazeBehaviour.Target && Player0.CurrentGazeBehaviour.Target == currentTarget && Player1.CurrentGazeBehaviour.Target == currentTarget)
                            {
                                JointAttention++;
                                writer.WriteLine("JA " + JointAttention);
                            }
                        }
                    }

                }
            }
        }

        
    }
}
