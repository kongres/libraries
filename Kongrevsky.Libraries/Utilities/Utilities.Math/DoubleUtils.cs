namespace Utilities.Math
{
    using System;

    public static class DoubleUtils
    {
        public static bool IsCloseToInt(this double d, double accuracy = 0.000001)
        {
            return Math.Abs(d - (int)d) < accuracy;
        }
    }
}