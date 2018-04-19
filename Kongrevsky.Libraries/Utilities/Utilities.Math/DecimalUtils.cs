namespace Utilities.Math
{
    using System;

    public static class DecimalUtils
    {
        public static bool LessThan(this decimal decimal1, decimal decimal2, int precision = 2)
        {
            return Math.Round(decimal1 - decimal2, precision) < 0;
        }

        public static bool LessThanOrEqualTo(this decimal decimal1, decimal decimal2, int precision = 2)
        {
            return Math.Round(decimal1 - decimal2, precision) <= 0;
        }

        public static bool GreaterThan(this decimal decimal1, decimal decimal2, int precision = 2)
        {
            return Math.Round(decimal1 - decimal2, precision) > 0;
        }

        public static bool GreaterThanOrEqualTo(this decimal decimal1, decimal decimal2, int precision = 2)
        {
            return Math.Round(decimal1 - decimal2, precision) >= 0;
        }

        public static bool AlmostEquals(this decimal decimal1, decimal decimal2, int precision = 2)
        {
            return Math.Round(decimal1 - decimal2, precision) == 0;
        }
    }
}
