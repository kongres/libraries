namespace Kongrevsky.Utilities.DateTime
{
    #region << Using >>

    using System;
    using System.Linq;

    #endregion

    public static class TimeSpanUtils
    {
        /// <summary>
        /// Parses TimeSpan from string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TimeSpan Parse(string str)
        {
            try
            {
                var list = str.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Select(int.Parse).ToList();

                switch (list.Count())
                {
                    case 0:
                        return new TimeSpan();
                    case 1:
                        return new TimeSpan(list[0]);
                    case 3:
                        return new TimeSpan(list[0], list[1], list[2]);
                    case 4:
                        return new TimeSpan(list[0], list[1], list[2], list[3]);
                    case 5:
                        return new TimeSpan(list[0], list[1], list[2], list[3], list[4]);
                }
            }
            catch (Exception) { }

            return new TimeSpan();
        }
    }
}