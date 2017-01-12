using System;
using NLog;

namespace Logger
{
    public class NLogAdapter : ILogAdapter
    {
        private readonly NLog.Logger logger;
        private static readonly Lazy<NLogAdapter> adapter = new Lazy<NLogAdapter>(() => new NLogAdapter());

        public static NLogAdapter Logger => adapter.Value;

        private NLogAdapter()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

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
