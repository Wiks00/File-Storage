using System;
using NLog;

namespace Logger
{
    public class NLogAdapter : ILogAdapter
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public void Error(Exception error, string message)
        {
            logger.Error(error, message);
        }

        public void Info(string message)
        {
            logger.Info(message);
        }
    }
}
