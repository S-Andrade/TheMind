﻿using System;
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
        private string RANDOM1;
        private string RANDOM2;
        private string RANDOM3;
        private string RANDOM4;
        private string RANDOM5;
        private string target;
        private int AVG_DUR_SCREEN;
        private int AVG_DUR_PLAYER;
        private Stopwatch RandomStopWatch;
        private int nextTimeToRandom;
        private int gazeTime;
        private string nextTarget;

        public RandomGazeController(AutonomousAgent thalamusClient) : base(thalamusClient)
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
            targets = new string[] { PLAYER_A, PLAYER_B, SCREEN, TABLET};
            target = SCREEN;
            RandomStopWatch = new Stopwatch();
            nextTimeToRandom = -1;
            gazeTime = 2000;
            nextTarget = RANDOM5;
        }

        
        public override void Update()
        {
            Console.WriteLine("Control");
            while (true)
            {
                if (SessionStarted)
                {
                    if (currentGazeDuration.ElapsedMilliseconds >= 1000)
                    {  
                        string target = "";

                        if (aa.lookatfront)
                        {
                            target = FRONT;
                        }

                         else if (aa._gameState == GameState.Game || aa._gameState == GameState.Waiting || aa._gameState == GameState.NextLevel)
                        {
                            if (nextTimeToRandom == -1)
                            {
                                RandomStopWatch.Restart();
                                nextTimeToRandom = random.Next(3500,5500);
                                gazeTime = random.Next(2000, 5000);
                                List<string> r = new List<string> { RANDOM1,RANDOM2,RANDOM3,RANDOM4,RANDOM5};
                                nextTarget = r[random.Next(r.Count)];

                            }
                            if (RandomStopWatch.IsRunning && RandomStopWatch.ElapsedMilliseconds >= nextTimeToRandom)
                            {
                                target = nextTarget;
                                
                            }
                            if (RandomStopWatch.IsRunning && RandomStopWatch.ElapsedMilliseconds >= gazeTime+nextTimeToRandom)
                            {
                                target = SCREEN;
                                RandomStopWatch.Stop();
                                nextTimeToRandom = -1;
                            }
                        }
                       

                        aa.TMPublisher.GazeBehaviourFinished("player2", currentTarget, (int) aa.SessionStartStopWatch.ElapsedMilliseconds);
                        currentTarget = target;
                        //Console.WriteLine("------------------------ gaze at " + currentTarget);
                        aa.TMPublisher.GazeAtTarget(currentTarget);
                        currentGazeDuration.Restart();
                        aa.TMPublisher.GazeBehaviourStarted("player2", currentTarget, (int) aa.SessionStartStopWatch.ElapsedMilliseconds);
                        

                        using (StreamWriter sw = File.AppendText("C:\\Users\\sandr\\Desktop\\the-mind-main\\timestampRandom.txt"))
                        {
                            sw.WriteLine(DateTime.Now + " P0 " + Player0.CurrentGazeBehaviour.Target + " P1 " + Player1.CurrentGazeBehaviour.Target + " P2 " + currentTarget + " " + aa._gameState);
                        }


                    }

                }
            }
        }
    }
}
