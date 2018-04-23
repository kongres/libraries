namespace Kongrevsky.Utilities.Math
{
    #region << Using >>

    using System;

    #endregion

    public static class DoubleUtils
    {
        /// <summary>
        /// Detects if Double is close to Int with specified accuracy
        /// </summary>
        /// <param name="d"></param>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static bool IsCloseToInt(this double d, double accuracy = 0.000001)
        {
            return Math.Abs(d - (int)d) < accuracy;
        }
    }
}