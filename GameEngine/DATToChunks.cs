using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GameEngine
{
    /// <summary>
    /// Load the DAT game file and break it into tokens which can be either numbers or string
    /// </summary>
    public static class DATToChunks
    {
        private static string file = null;
        private static int pos = 0;

        public static void Load(string pFile)
        {
            pos = 0;
            file = File.ReadAllText(pFile).Trim();
        }

        static string[] le = new string[] { "\n", "\r" };


        public static bool EOF { get { return !(pos < file.Length); } }

        /// <summary>
        /// Get the required number of DAT chunks as string
        /// </summary>
        /// <param name="pCount">Number of tokens to return</param>
        /// <returns>string array</returns>
        public static string[] getTokens(int pCount)
        {
            string[] retval = new string[pCount];
            int ctr = 0;

            while (ctr < pCount)
            {
                switch (file[pos].ToString())
                {
                    case "\""://encountered the begining of a string, loop until another inverted comma is found
                        do
                        {
                            retval[ctr] += file[pos];
                            pos++;
                        } while (!EOF && file[pos].ToString() != "\"");
                        break;

                    default://must be at a number
                        do
                        {
                            retval[ctr] += file[pos];
                            pos++;
                        } while (!EOF && file[pos] != '\n');
                        break;
                }

                do
                {
                    pos++;
                } while (!EOF && le.Contains(file[pos].ToString()));

                retval[ctr] = retval[ctr].Trim(new char[] { ' ', '"', '\r', '\n' });

                ctr++;
            }
            return retval;
        }

        /// <summary>
        /// Get the required number of DAT tokens as int
        /// </summary>
        /// <param name="pCount"></param>
        /// <returns></returns>
        public static int[] GetTokensAsInt(int pCount)
        {
            return getTokens(pCount).Select(i => Convert.ToInt32(i)).ToArray();
        }

    }
}
