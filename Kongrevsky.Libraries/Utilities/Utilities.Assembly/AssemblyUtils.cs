namespace Utilities.Assembly
{
    #region << Using >>

    using System;
    using System.IO;
    using System.Reflection;

    #endregion

    public static class AssemblyUtil
    {
        /// <summary>
        /// Get last building DateTime
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static DateTime? GetBuildDateTime(this Assembly assembly)
        {
            try
            {
                var buildDate = new FileInfo(assembly.Location).LastWriteTime;

                return buildDate;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get build number
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static int GetBuildNumber(this Assembly assembly)
        {
            var date = assembly.GetBuildDateTime();

            return date == null ? 0 : (date.Value - new DateTime(1970, 1, 1)).Days;
        }

        //TODO: add reverse method for GetBuildNumber - send: BuildNumber, get: BuildDateTime
    }
}