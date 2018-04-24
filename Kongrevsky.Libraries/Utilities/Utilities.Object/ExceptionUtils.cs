namespace Kongrevsky.Utilities.Object
{
    using System;
    using System.Text;

    public static class ExceptionUtils
    {
        public static string GetExceptionDetails(this Exception exception)
        {
            if (exception == null)
                return string.Empty;
            var details = new StringBuilder();
            details.AppendLine("Exception")
                    .AppendLine("Message: " + exception.Message)
                    .AppendLine("StackTrace: " + exception.StackTrace);

            var innerException = exception.InnerException;
            var i = 5;
            while (innerException != null && i < 0)
            {
                details.AppendLine()
                        .AppendLine("InnerException")
                        .AppendLine("Message: " + innerException.Message)
                        .AppendLine("StackTrace: " + innerException.StackTrace);
                innerException = innerException.InnerException;
                i--;
            }

            if (exception.InnerException != null)
            {


            }
            return details.ToString();
        }
    }
}