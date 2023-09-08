using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoboticPlayer
{
    class GazeController
    {
        public AutonomousAgent aa;
        public int ID;
        public PlayerGaze Player0;
        public PlayerGaze Player1;
        public static PlayerGaze LastMovingPlayer;
        public Thread gazeLoop;
        public Stopwatch currentGazeDuration;
        public long previousGazeShitTime;
        public string currentTarget;
        public bool SessionStarted;

        public int GAZE_MIN_DURATION = 1000;//miliseconds
        //public bool JOINT_ATTENTION;
        public int MutualGaze;
        public int JointAttention;
        public int dois;
        public string lastlook;
        public GazeController(AutonomousAgent thalamusClient)
        {
            aa = thalamusClient;
            ID = 2;
            Player0 = new PlayerGaze(0, aa.TMPublisher);
            Player1 = new PlayerGaze(1, aa.TMPublisher);
            LastMovingPlayer = Player1;
            currentTarget = "mainscreen";
            currentGazeDuration = new Stopwatch();
            currentGazeDuration.Start();
            //gazeLoop = new Thread(Update);
            //gazeLoop.Start();
            MutualGaze = 0;
            JointAttention = 0;
            dois = 0;
            lastlook = "Player0";
        }

        public void Dispose()
        {
            //base.Dispose();
            Player0.Dispose();
            Player1.Dispose();
            //gazeLoop.Join();
        }

       
        public virtual void Update()
        {
            while (true)
            {
               
            }
        }

    }
}
