using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Areas.Admin.Helper
{
    public static class CompareStringHelper
    {
        public static double CompareString(string str1, string str2)
        {
            List<string> pairs1 = WordLetterPair(str1.ToUpper());
            List<string> pairs2 = WordLetterPair(str2.ToUpper());
            int union = pairs1.Count + pairs2.Count;
            int intersection = 0;

            foreach (string p1 in pairs1)
            {
                foreach (string p2 in pairs2)
                {
                    if (p1 == p2)
                    {
                        intersection++;
                        pairs2.Remove(p2);
                        break;
                    }
                }
            }
            return (2.0*intersection)/union;
        }

        /// <summary>
        /// Split the input string into words and get adjacent letter pairs of each word
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>A list contains lists of adjacent letter pairs</returns>
        private static List<string> WordLetterPair(string str)
        {
            var allPairs = new List<string>();
            string[] words = str.Split(' ');
            foreach (string word in words)
            {
                List<string> tmp = LetterPair(word);
                foreach (string s in tmp)
                {
                    allPairs.Add(s);
                }
            }
            return allPairs;
        }

        /// <summary>
        /// Divide the input string into pairs of adjacent letter
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>An list of adjacent letter pairs contained in the input string</returns>
        private static List<string> LetterPair(string str)
        {
            var pairs = new List<string>();
            for (int i = 0; i < str.Length - 1; i++)
            {
                string tmp = str.Substring(i, 2);
                pairs.Add(tmp);
            }
            return pairs;
        }
    }
}