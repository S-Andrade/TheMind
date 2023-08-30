using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoboticPlayer
{
    class ReactiveGazeController : GazeController 
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
        private int lookplayer0;
        private int lookplayer1;
        

        public ReactiveGazeController(AutonomousAgent thalamusClient) : base(thalamusClient)
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
            lookplayer0 = 0;
            lookplayer1 = 0; 
        }

        public override void Update()
        {
            Console.WriteLine("Reactive");
            while (true)
            {
                if (SessionStarted)
                {
                    if (currentGazeDuration.ElapsedMilliseconds >= GAZE_MIN_DURATION & Player0.SessionStarted & Player1.SessionStarted)
                    {
                        string target = "";

                        //Task-oriented behaviour//
                        
                        if (aa.lookatplayer != -1)
                        {
                            if (aa.lookatplayer == 0)
                            {
                                target = PLAYER_A;
                                Console.Write("Control>");
                            }
                            if (aa.lookatplayer == 1)
                            {
                                target = PLAYER_B;
                                Console.Write("Control>");
                            }
                        }
                        else if (aa.lookattablet)
                        {
                            target = TABLET;
                            Console.Write("Control>");
                        }
                        else if (aa.lookatfront)
                        {
                            target = FRONT;
                            Console.Write("Control>");
                        }
                       
                        else if (aa._gameState == GameState.Game)
                        {
                             //Mutual gaze//

                                //player0 looks at robot
                                //robot looks at player0
                            if (Player0.IsGazingAtRobot() && (Player1.CurrentGazeBehaviour.Target == "mainscreen" || Player1.CurrentGazeBehaviour.Target == "elsewhere"))
                            {
                                target = PLAYER_A;
                                lookplayer0 += 1;
                            }
                            //player1 looks at robot
                            //robots looks at player1
                            else if (Player1.IsGazingAtRobot() && (Player0.CurrentGazeBehaviour.Target == "mainscreen" || Player0.CurrentGazeBehaviour.Target == "elsewhere"))
                            {
                                target = PLAYER_B;
                                lookplayer1 += 1;
                            }
                            //player0 and player1 look at eachother
                            //look at the least gazed
                            else if (Player0.CurrentGazeBehaviour.Target == "player1" && Player1.CurrentGazeBehaviour.Target == "player0")
                            {
                                if (lookplayer0 > lookplayer1)
                                {
                                    target = PLAYER_B;
                                    lookplayer1 += 1;
                                }

                                if (lookplayer1 > lookplayer0)
                                {
                                    target = PLAYER_A;
                                    lookplayer0 += 1;
                                }

                            }
                            //player0 and player1 look at the robot
                            //look at the least gazed
                            else if (Player0.IsGazingAtRobot() && Player1.IsGazingAtRobot())
                            {
                                if (lookplayer0 > lookplayer1)
                                {
                                    target = PLAYER_B;
                                    lookplayer1 += 1;
                                }

                                if (lookplayer1 > lookplayer0)
                                {
                                    target = PLAYER_A;
                                    lookplayer0 += 1;
                                }
                            }


                            //Joint Attention//

                            //player0 looks at player1
                            //robot looks at player1
                            else if (Player0.CurrentGazeBehaviour.Target == "player1" & (Player1.CurrentGazeBehaviour.Target == "mainscreen" || Player1.CurrentGazeBehaviour.Target == "elsewhere"))
                            {
                                target = PLAYER_B;
                                lookplayer1 += 1;
                            }
                            //player1 looks at player0
                            //robot looks at player0
                            else if (Player1.CurrentGazeBehaviour.Target == "player0" && (Player0.CurrentGazeBehaviour.Target == "mainscreen" || Player0.CurrentGazeBehaviour.Target == "elsewhere"))
                            {
                                target = PLAYER_A;
                                lookplayer0 += 1;
                            }
                            //player0 looks at the robot and player1 looks at player0
                            //robot looks at player0
                            else if (Player0.IsGazingAtRobot() && Player1.CurrentGazeBehaviour.Target == "player0")
                            {
                                target = PLAYER_A;
                                lookplayer0 += 1;
                            }
                            //player1 looks at the robot and player0 looks at player1
                            //robot looks at player1
                            else if (Player1.IsGazingAtRobot() && Player0.CurrentGazeBehaviour.Target == "player1")
                            {
                                target = PLAYER_B;
                                lookplayer1 += 1;
                            }
                            //player0 and player1 look at the mainscreen
                            //robot look at mainscreen
                            else if (Player0.CurrentGazeBehaviour.Target == "mainscreen" && Player1.CurrentGazeBehaviour.Target == "mainscreen")
                            {
                                target = SCREEN;
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
                          //  aa.TMPublisher.("player2", "neutral", 0, 0);
                        //}
                        


                        aa.TMPublisher.GazeBehaviourFinished("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);
                        currentTarget = target;
                        //Console.WriteLine("------------------------ gaze at " + currentTarget);
                        aa.TMPublisher.GazeAtTarget(currentTarget);
                        currentGazeDuration.Restart();
                        aa.TMPublisher.GazeBehaviourStarted("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);

                        Console.WriteLine(currentTarget);
                        Console.WriteLine(lookplayer0);
                        Console.WriteLine(lookplayer1);


                        using (StreamWriter writer = new StreamWriter("C:\\Users\\sandr\\Desktop\\the-mind-main\\reactive.txt"))
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

                    
                    /*//if (currentGazeDuration.ElapsedMilliseconds >= GAZE_MIN_DURATION && Player0.SessionStarted && Player0.CurrentGazeBehaviour != null && Player1.SessionStarted && Player1.CurrentGazeBehaviour != null)
                    if (currentGazeDuration.ElapsedMilliseconds >= GAZE_MIN_DURATION && Player0.SessionStarted && Player1.SessionStarted && Player1.CurrentGazeBehaviour != null)
                    {
                        //reactive
                        if (LastMovingPlayer.IsGazingAtRobot() && currentTarget != LastMovingPlayer.Name)
                        {
                            Console.WriteLine("------------------------ gaze back " + LastMovingPlayer.Name);
                            aa.TMPublisher.GazeBehaviourFinished("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);
                            currentTarget = LastMovingPlayer.Name;
                            aa.TMPublisher.GazeAtTarget(LastMovingPlayer.Name);
                            currentGazeDuration.Restart();
                            aa.TMPublisher.GazeBehaviourStarted("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);
                            NextPractiveBehaviour(currentGazeDuration.ElapsedMilliseconds);
                        }
                        else if (!LastMovingPlayer.IsGazingAtRobot() && LastMovingPlayer.CurrentGazeBehaviour.Target != "elsewhere" && currentTarget != LastMovingPlayer.CurrentGazeBehaviour.Target)
                        {
                            Console.WriteLine("------------------------ gaze at where " + LastMovingPlayer.Name + " is gazing " + LastMovingPlayer.CurrentGazeBehaviour.Target);
                            aa.TMPublisher.GazeBehaviourFinished("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);
                            currentTarget = LastMovingPlayer.CurrentGazeBehaviour.Target;
                            aa.TMPublisher.GazeAtTarget(LastMovingPlayer.CurrentGazeBehaviour.Target);
                            currentGazeDuration.Restart();
                            aa.TMPublisher.GazeBehaviourStarted("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);
                            NextPractiveBehaviour(currentGazeDuration.ElapsedMilliseconds);
                        }
                        else if (!LastMovingPlayer.IsGazingAtRobot() && LastMovingPlayer.CurrentGazeBehaviour.Target != "elsewhere" && currentTarget != "mainscreen")
                        {
                            Console.WriteLine("------------------------ mutual gaze break");
                            aa.TMPublisher.GazeBehaviourFinished("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);
                            currentTarget = "mainscreen";
                            aa.TMPublisher.GazeAtTarget("mainscreen");
                            currentGazeDuration.Restart();
                            aa.TMPublisher.GazeBehaviourStarted("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);
                            NextPractiveBehaviour(currentGazeDuration.ElapsedMilliseconds);
                        }


                        //proactive
                        if (PROACTIVE_NEXT_SHIFT != -1 && currentGazeDuration.ElapsedMilliseconds >= PROACTIVE_NEXT_SHIFT)
                        {
                            Console.WriteLine(">>>>> PROACTIVE <<<<< gaze at " + PROACTIVE_NEXT_TARGET + " prev-dur " + currentGazeDuration.ElapsedMilliseconds);
                            aa.TMPublisher.GazeBehaviourFinished("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);
                            currentTarget = PROACTIVE_NEXT_TARGET;
                            aa.TMPublisher.GazeAtTarget(PROACTIVE_NEXT_TARGET);
                            currentGazeDuration.Restart();
                            aa.TMPublisher.GazeBehaviourStarted("player2", currentTarget, (int)aa.SessionStartStopWatch.ElapsedMilliseconds);
                            NextPractiveBehaviour(currentGazeDuration.ElapsedMilliseconds);
                        }
                    }*/
                }
            }
        }
    }
}
