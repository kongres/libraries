namespace Kongrevsky.Infrastructure.Web.ActionLogger.Models
{
    public class ActionLog
    {
        public ActionLogType LogType { get; set; }

        public string Name { get; set; }

        public string InputData { get; set; }

        public string OutputData { get; set; }
    }
}