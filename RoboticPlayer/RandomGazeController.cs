using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheMindThalamusMessages;

namespace RoboticPlayer
{
    class RandomGazeController : GazeController
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

        public RandomGazeController(AutonomousAgent thalamusClient) : base(thalamusClient)
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
            targets = new string[] { PLAYER_A, PLAYER_B, SCREEN, TABLET};
            target = SCREEN;
        }

        
        public override void Update()
        {
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
                            }
                            if (aa.lookatplayer == 1)
                            {
                                target = PLAYER_B;
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
                        else if (aa.lookrandom)
                        {
                            target = RANDOM;
                        }
                        else
                        {
                            target = SCREEN;
                        }
                        

                        if (currentTarget == TABLET)
                        {
                            aa.TMPublisher.SetPosture("player2", "neutral", 0, 0);
                        }

                        aa.TMPublisher.GazeBehaviourFinished("player2", currentTarget, (int) aa.SessionStartStopWatch.ElapsedMilliseconds);
                        currentTarget = target;
                        //Console.WriteLine("------------------------ gaze at " + currentTarget);
                        aa.TMPublisher.GazeAtTarget(currentTarget);
                        currentGazeDuration.Restart();
                        aa.TMPublisher.GazeBehaviourStarted("player2", currentTarget, (int) aa.SessionStartStopWatch.ElapsedMilliseconds);
                        
                    }

                }
            }
        }
    }
}
