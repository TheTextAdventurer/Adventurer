using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using GameEngine;


namespace Adventurer
{
    class Program
    {
        /// <summary>
        /// View of curent room
        /// </summary>
        static string _GameView;

        /// <summary>
        /// Message returned by game
        /// </summary>
        static string _GameMessage;

        /// <summary>
        /// Display the turn counter, set by flag
        /// </summary>
        static bool _TurnCounter;

        /// <summary>
        /// Is robot mode enabled
        /// </summary>
        static bool _Robot;

        /// <summary>
        /// Robot timer
        /// </summary>
        static Stopwatch _RobotTimer;

        /// <summary>
        /// User input, recorded after keypress
        /// </summary>
        static bool _NewInput = false;

        /// <summary>
        /// Name of robot output file
        /// </summary>
        static string _RobotFile = null;

        /// <summary>
        /// Robot data stored here
        /// </summary>
        static XElement _RobotOutput;

        //load save game/load game/output friendly xml/output raw xml/display turn counter/show help/robot/output as commented DAT
        static string[] flags = { "-l", "-g", "-f", "-r", "-t","-h","-b","-c" };

        /// <summary>
        /// Returns the argument provided with the specified flag
        /// </summary>
        /// <param name="pFlag">Flag being sought</param>
        /// <param name="pArgs">Complete list of arguments</param>
        /// <returns>Flag argument</returns>
        static string getFlagArg(string pFlag, string[] pArgs)
        {
            string match = pArgs.SkipWhile(a => !a.StartsWith(pFlag))
                        .FirstOrDefault();

            if (match == null)
                return null;
            else
                return match.Substring(pFlag.Length);

        }

        static void Main(string[] args)
        {
            Console.Clear();           
            string arg;
            string arg1;
            string userInput = null;
            

            try
            {
                #region Process arguments

                //no args provided or no recognised args
                if (args.Length == 0
                        || args.Select(a => a.Length > 1 ? a.Substring(0, 2) : a).ToArray().Intersect(flags).Count() == 0
                        || getFlagArg(flags[5], args) != null)
                {
                    OutputHelp();
                    return;
                }
                else if ((arg = getFlagArg(flags[3], args)) != null) //raw XML output of specified game file
                {
                    var g = GameData.Load(arg);
                    g.SaveAsUncommentedXML(arg);
                    return;
                }
                else if ((arg = getFlagArg(flags[2], args)) != null) //formatted output of specified game file
                {
                    var g = GameData.Load(arg);
                    g.SaveAsCommentedXML();
                    return;
                }
                else if ((arg = getFlagArg(flags[7], args)) != null) //formatted output of specified game file
                {
                    GameData.SaveAsCommentedDat(arg);
                    return;
                }

                arg = getFlagArg(flags[1], args);//Game to load
                arg1 = getFlagArg(flags[0], args);//Save game to restore
                _TurnCounter = getFlagArg(flags[4], args) != null;
                if (arg != null)
                {

                    Advent.RoomView += Advent_RoomView;
                    Advent.GameMessages += Advent_GameMessages;
                    if (arg1 != null)
                    {
                        Advent.RestoreGame(arg, arg1);  //restore save game
                    }
                    else
                        Advent.LoadGame(arg);   //just load
                }
                else
                {
                    Console.WriteLine("ERROR: When using -l must specify game to load with -g");
                    return;
                }


                //display turn counter
                if (!_TurnCounter)
                    Console.Title = "Playing: " + Advent.GameName;

                //Enable robot
                _Robot = (_RobotFile = getFlagArg(flags[6], args)) != null;
                if (_Robot)
                {
                    Console.CancelKeyPress += Console_CancelKeyPress;
                    _RobotTimer = new Stopwatch();
                    _RobotOutput = new XElement("AdventurerRobot", new XAttribute("Game", Advent.GameName));
                    
                }
                #endregion


                do
                {

                    Console.Write(Advent.PlayerPrompt);

                    if (_Robot)
                    {
                        _RobotTimer.Start();
                        _NewInput = true;
                    }

                    Advent.ProcessText(userInput = Console.ReadLine().Trim());

                    if (_Robot)
                    {
                        _RobotTimer.Stop();
                        _NewInput = true;
                        _RobotOutput.Add(new XElement("Turn", new XAttribute("Input", userInput), new XAttribute("Delay", _RobotTimer.ElapsedMilliseconds)));
                        _RobotTimer.Reset();
                    }

                } while (!Advent.ISGameOver);

                Output();
            }
            catch (Exception e)
            {
                ErrorBox(6, 6, 70, 10, e.Message);
            }

            if (_Robot)
                _RobotOutput.Save(_RobotFile +".robot");

            //write the final bar
            string exitmsg = "-Press enter to exit-";
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(exitmsg + new string(' ', Console.WindowWidth - exitmsg.Length));
            Console.SetCursorPosition(exitmsg.Length + 1, Console.CursorTop - 1);
            Console.ResetColor();
            Console.Read();
            Console.Clear();
        }

        /// <summary>
        ///  Record the time at which the first keypress occurs. Only used if robot mode enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (_NewInput)
            {
                _RobotTimer.Stop(); //The time at which the first keypress of the new turn begins
                _NewInput = false;
            }
        }

        /// <summary>
        /// Output the entire world view
        /// </summary>
        private static void Output()
        {
            Console.Clear();
            if (_TurnCounter)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;               
                Console.WriteLine(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 0);
                Console.Write("Playing: " + Advent.GameName);
                Console.SetCursorPosition(Console.WindowWidth - 15, 0);
                Console.Write("Turns: {0}", Advent.TurnCounter);
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine();
            }


            Console.WriteLine(_GameView);
            Console.WriteLine();

            Console.WriteLine(_GameMessage);
            Console.WriteLine();            
        }

        /// <summary>
        /// To prevent a word being displayed over two lines, break the string
        /// into individual words and print as many as fit onto a line
        /// </summary>
        /// <param name="pText"></param>
        private static void outputLine(string pText)
        {
            if (pText == null)
                return;

            if (pText.Length < Console.WindowWidth)
                Console.WriteLine(pText);
            else
            {
                string line = "";
                foreach (string word in pText.Split(new char[] { ' ' }))
                {
                    if ((line.Length + word.Length) < Console.WindowWidth)
                        line += (word + " ");
                    else
                    {
                        Console.WriteLine(line);
                        line = word;
                    }
                }
            }
        }

        /// <summary>
        /// Display help screen
        /// </summary>
        private static void OutputHelp()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Adventurer.exe");
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("A C# Scott Adams adventure game emulator for games in the SCOTTFREE format.");
            Console.WriteLine();
            Console.Write("Website: ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("https://github.com/TheTextAdventurer/Adventurer");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Exe arguments: ");
            Console.WriteLine("\t-g\tLoad DAT file -gAdv01.dat");
            Console.WriteLine("\t-g\tSpecify game save file -lAdv01.sav - must be used with -g");
            Console.WriteLine("\t-t\tDisplay turn counter in game");
            Console.WriteLine("\t-f\tOutput specified game in commented XML -fAdv01.dat");
            Console.WriteLine("\t-r\tOutput specified game in uncommented XML -rAdv01.dat");
            Console.WriteLine("\t-c\tOutput specified game in formatted dat -cAdv01.dat");
            Console.WriteLine("\t-h\tDisplay help");
            Console.WriteLine("\t-b\tRecord game, must specify output name -bRecorded");
            Console.WriteLine();
            Console.WriteLine("This application is distributed under the GNU GENERAL PUBLIC LICENSE");
            Console.Write("Website: ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("https://www.gnu.org/licenses/gpl-3.0.en.html");
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// A message has been sent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Advent_GameMessages(object sender, Advent.GameOuput e)
        {
            Debug.WriteLine("Advent_GameMessages: " + e.Message);
            if (e.Refresh)
                _GameMessage = e.Message;
            else
                _GameMessage += e.Message;
                Output();
        }



        /// <summary>
        /// Game view has been updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Advent_RoomView(object sender, Advent.GameOuput e)
        {
            _GameView = e.Message;
            Debug.WriteLine("Advent_RoomView");
            Output();
        }

        /// <summary>
        /// ErrorBox
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pWidth"></param>
        /// <param name="pHeight"></param>
        /// <param name="pEdge"></param>
        /// <param name="pMessage"></param>
        private static void ErrorBox(int pX, int pY, int pWidth, int pHeight, string pMessage)
        {

            char edge = '*';
            int y;

            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;


            //top of box
            Console.SetCursorPosition(pX, pY);
            Console.WriteLine(new string(edge, pWidth));
            Console.SetCursorPosition(pX + 4, pY);
            Console.WriteLine(" An error has occurred: ");

            //sides
            for (y = pY + 1; y <= pY + pHeight; y++)
            {
                Console.SetCursorPosition(pX, y);
                Console.WriteLine(edge + new string(' ', pWidth - 2) + edge);
            }

            //bottom
            Console.SetCursorPosition(pX, pY + pHeight);
            Console.WriteLine(new string(edge, pWidth));


            int x = pX + 2;
            y = pY + 2;

            //write out the message, a character at a time within the box
            pMessage.ToArray().All(c =>
            {
                x++;
                if (x > (pX + pWidth - 4))
                {
                    x = pX + 2;
                    y++;
                }
                Console.SetCursorPosition(x, y);
                Console.Write(c);
                return true;
            });

            Console.ResetColor();
        }
    }
}
