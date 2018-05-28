using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Adventurer
{
    class Program
    {
        static string _GameView;
        static string _GameMessage;
        static bool _TurnCounter;

        /*
         *  -l
         *  -g
         *  -x
         *  -x1
         * 
         */

        static string[] flags = { "-l", "-g", "-f", "-r", "-t","-h" };

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
            Advent.GameView += Advent_GameView;
            Advent.GameOutput += Advent_GameOutput;

            string arg;
            string arg1;

            if (args.Length == 0
                    || args.Select(a=> a.Length > 1 ? a.Substring(0,2) : a).ToArray().Intersect(flags).Count() == 0
                    || getFlagArg(flags[5], args) != null
                    )
            {
                OutputHelp();
                return;
            }
            else if ((arg=getFlagArg(flags[3], args)) != null) //raw XML output
            {
                Advent.LoadGame(arg);
                Advent.SaveAsUnFormattedXML(arg);
                return;
            }
            else if ((arg=getFlagArg(flags[2], args)) != null) //formatted output
            {
                Advent.LoadGame(arg);
                Advent.SaveAsFormattedXML();
                return;
            }




            arg = getFlagArg(flags[1], args);//load game
            arg1 = getFlagArg(flags[0], args);//restore save
            _TurnCounter = getFlagArg(flags[4], args) != null;
            if (arg != null)
            {
                if (arg1 != null)
                {
                    Advent.RestoreGame(arg, arg1);
                }
                else
                    Advent.LoadGame(arg);
            }
            else
            {
                Console.WriteLine("ERROR: Must specify game to load with -g");
                OutputHelp();
                return;
            }

            //
           



            try
            {


                if (!_TurnCounter)
                    Console.Title = "Playing: " + Advent.GameName;

                do
                {

                    Console.Write(Advent.PlayerPrompt);
                    Advent.ProcessText(Console.ReadLine().Trim());

                } while (!Advent.ISGameOver);

                Output();
            }
            catch (Exception e)
            {
                ErrorBox(6, 6, 70, 10, e.Message);
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
            Console.WriteLine("\t-r\tOutput specified game in XML -rAdv01.dat");
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
        private static void Advent_GameOutput(object sender, Advent.GameOuput e)
        {
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
        private static void Advent_GameView(object sender, Advent.GameOuput e)
        {
            _GameView = e.Message;

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
