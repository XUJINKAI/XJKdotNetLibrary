using System;
using System.Collections.Generic;
using System.Text;

namespace XJK
{
    public static class StringGenerator
    {
        public static string PercentageString(int Percentage, int length = 10, char left = '.', char right = ' ')
        {
            int leftcount = Percentage * length / 100;
            int rightcount = length - leftcount;
            return new string(left, leftcount) + new string(right, rightcount);
        }
    }
}
