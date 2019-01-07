using System;
using System.Linq;
using System.Diagnostics;
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

        static string _GameItems;

        /// <summary>
        /// Message returned by game
        /// </summary>
        static string _GameMessage;



        /// <summary>
        /// Display the turn counter, set by flag
        /// </summary>
        static bool _TurnCounter;



        //load save game/load game/output as xml/display turn counter/show help/output as commented DAT
        static readonly string[] flags = { "-l", "-g", "-x", "-t", "-h",  "-c" };

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
            Console.WindowWidth = 70;

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
                else if ((arg = getFlagArg(flags[2], args)) != null) //formatted output of specified game file
                {
                    var g = GameData.Load(arg);
                    g.SaveAsCommentedXML();
                    return;
                }
                else if ((arg = getFlagArg(flags[5], args)) != null) //formatted output of specified game file
                {
                    GameData.SaveAsCommentedDat(arg);
                    return;
                }

                arg = getFlagArg(flags[1], args);//Game to load
                arg1 = getFlagArg(flags[0], args);//Save game to restore
                _TurnCounter = getFlagArg(flags[3], args) != null;
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



                Console.Title = "Playing: " + Advent.GameName;


                #endregion


                do
                {

                    Console.Write(Advent.PlayerPrompt);
                                 
                    Advent.ProcessText(userInput = Console.ReadLine());

    
                } while (!Advent.ISGameOver);

                Output();
            }
            catch (Exception e)
            {
                ErrorBox(6, 6, 60, 10, e.Message);
            }


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
        /// Recieved an update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Advent_RoomView(object sender, Advent.Roomview e)
        {
            _GameView = e.View;
            _GameItems = e.Items;
            Output();
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
            
            if (_GameItems != null && _GameItems.Length > 0)
            {
                Console.WriteLine();
                if (_GameItems.Length < Console.WindowWidth)
                    Console.WriteLine(_GameItems);
                else
                {
                    //break the item list into words and build a string that is
                    //less than the console width ensure no words are broken over
                    //two lines
                    string line = "";
                    foreach (string word in _GameItems.Split(new char[] { ' ' }))
                    {
                        if ((line.Length + word.Length) < Console.WindowWidth)
                            line += (word + " ");
                        else
                        {
                            Console.WriteLine(line);
                            line = word + " ";
                        }
                    }
                    Console.WriteLine(line);
                }
            }

            Console.WriteLine();
            Console.WriteLine(_GameMessage.TrimEnd());
            Console.WriteLine();
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
            Console.WriteLine("\t-f\tOutput specified game in XML -xAdv01.dat");
            Console.WriteLine("\t-c\tOutput specified game in formatted dat -cAdv01.dat");
            Console.WriteLine("\t-h\tDisplay help");
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
            if (e.Refresh)
                _GameMessage = e.Message;
            else
                _GameMessage += e.Message;
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
