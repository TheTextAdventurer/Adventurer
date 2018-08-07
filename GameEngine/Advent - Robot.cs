using System.Diagnostics;
using System.Xml.Linq;

namespace GameEngine
{

    static partial class Advent
    {

        /// <summary>
        /// Is robot mode enabled
        /// </summary>
        private static bool _Robot;


        /// <summary>
        /// Robot timer
        /// </summary>
        static Stopwatch _RobotTimer;
        private static string _RobotFile;

        /// <summary>
        /// Robot data stored here
        /// </summary>
        static XElement _RobotOutput;

        public  static void RobotInit(string pRobotSave)
        {
            _Robot = true;
            _RobotFile = pRobotSave;
            _RobotTimer = new Stopwatch();
            _RobotOutput = new XElement("AdventurerRobot", new XAttribute("Game", Advent.GameName));
        }

        public static void RobotSave()
        {
            if (_Robot)
                _RobotOutput.Save(_RobotFile + ".robot");
        }

        public static void RobotBegin()
        {
            if (!_Robot)
                return;

            _RobotTimer.Start();
        }

        public static void RobotEnd(string pInput)
        {
            if (!_Robot)
                return;

            _RobotTimer.Stop();
            _RobotOutput.Add(new XElement("Turn", new XAttribute("Input", pInput), new XAttribute("Delay", _RobotTimer.ElapsedMilliseconds)));
            _RobotTimer.Reset();
        }

    }
}