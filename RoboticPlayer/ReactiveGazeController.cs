using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoboticPlayer
{
    class ReactiveGazeController : GazeController 
    {
      
        public ReactiveGazeController(AutonomousAgent thalamusClient) : base(thalamusClient)
        {
            
        }

        public override void Update()
        {
            while (true)
            {
                if (SessionStarted)
                {
                    if (currentGazeDuration.ElapsedMilliseconds >= GAZE_MIN_DURATION && Player0.SessionStarted && Player1.SessionStarted)
                    {
                        //private moments
                        //player0 looks at player1
                        if (Player0.CurrentGazeBehaviour.Target == "player1" && (Player1.CurrentGazeBehaviour.Target == "mainscreen" || Player1.CurrentGazeBehaviour.Target == "elsewhere")) 
                        { 
                            
                        }
                        //player0 looks at robot
                        if (Player0.IsGazingAtRobot() && (Player1.CurrentGazeBehaviour.Target == "mainscreen" || Player1.CurrentGazeBehaviour.Target == "elsewhere")) { }

                        //player1 looks at player0
                        if (Player1.CurrentGazeBehaviour.Target == "player0" && (Player0.CurrentGazeBehaviour.Target == "mainscreen" || Player0.CurrentGazeBehaviour.Target == "elsewhere")) { }
                        //player1 looks at robot
                        if (Player1.IsGazingAtRobot() && (Player0.CurrentGazeBehaviour.Target == "mainscreen" || Player0.CurrentGazeBehaviour.Target == "elsewhere")) { }


                        //public moments
                        //player0 and player1 look at eachother
                        if (Player0.CurrentGazeBehaviour.Target == "player1" && Player1.CurrentGazeBehaviour.Target == "player0") { }

                        //player0 and player1 look at the robot
                        if (Player0.IsGazingAtRobot() && Player1.IsGazingAtRobot()) { }

                        //player0 and player1 look at the mainscreen
                        if (Player0.CurrentGazeBehaviour.Target == "mainscreen" && Player1.CurrentGazeBehaviour.Target == "mainscreen") { }


                        //tricky moments
                        //player0 looks at the robot and player1 looks at player0
                        if (Player0.IsGazingAtRobot() && Player1.CurrentGazeBehaviour.Target == "player0") { }
                        //player1 looks at the robot and player0 looks at player1
                        if (Player1.IsGazingAtRobot() && Player0.CurrentGazeBehaviour.Target == "player1") { }



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
