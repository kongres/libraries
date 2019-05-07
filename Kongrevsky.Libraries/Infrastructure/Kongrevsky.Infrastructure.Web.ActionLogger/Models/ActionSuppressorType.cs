namespace Kongrevsky.Infrastructure.Web.ActionLogger.Models
{
    #region << Using >>

    using System;

    #endregion

    [Flags]
    public enum ActionSuppressorType
    {
        No = 0,

        Info = 1,

        BadRequest = 2,

        Warning = 4,

        Error = 8,

        All = Info | BadRequest | Warning | Error
    }
}