﻿using System;
using System.Linq;

namespace XJK
{
    public static class Helper
    {
        public static string PercentageToText(int Percentage, int length = 10, char left = '.', char right = ' ')
        {
            int leftcount = Percentage * length / 100;
            int rightcount = length - leftcount;
            return new string(left, leftcount) + new string(right, rightcount);
        }
        
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int RandomInt(int Max) => RandomInt(0, Max);

        public static int RandomInt(int Min, int Max)
        {
            return random.Next(Min, Max);
        }

        public static double RandomDouble(double Max) => RandomDouble(0, Max);

        public static double RandomDouble(double Min, double Max)
        {
            return (Max - Min) * random.NextDouble() + Min;
        }

        public static double RandomDouble(double Min, double Max, int Precision)
        {
            double result = RandomDouble(Min, Max);
            return Math.Round(result, Precision);
        }
    }
}
