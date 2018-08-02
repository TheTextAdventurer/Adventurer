using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GameEngine
{
    /// <summary>
    /// Load the DAT game file and break it into tokens which can be either numbers or string
    /// </summary>
    public static class DATToChunks
    {

        private static string[] tokens = null;

        public static void Load(string pFile)
        {
            var r = new Regex
                            (
                                "([\"][^\"]*?[\"])" //string
                                + "|(/\\*(?:(?!\\*/).)*\\*/)"   //multiline comment
                                + "|(\\d+)" //number
                                , RegexOptions.Singleline
                            );

            tokens = r.Matches(File.ReadAllText(pFile))
                        .Cast<Match>()
                        .Where(m => !m.ToString().StartsWith("/"))//remove the comments
                        .Select(m => m.ToString().Trim(new char[] { '"' }))
                        .ToArray();
        }


        //private static string file = null;
        private static int pos = 0;

        //public static void Load(string pFile)
        //{
        //    pos = 0;
        //    file = File.ReadAllText(pFile).Trim();
        //}

        //static string[] le = new string[] { "\n", "\r" };


        public static bool EOF { get { return !(pos < tokens.Length); } }

        /// <summary>
        /// Get the required number of DAT chunks as string
        /// </summary>
        /// <param name="pCount">Number of tokens to return</param>
        /// <returns>string array</returns>
        public static string[] getTokens(int pCount)
        {
            string[] retval =
                tokens.Skip(pos).Take(pCount).ToArray();

            pos += pCount;

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
