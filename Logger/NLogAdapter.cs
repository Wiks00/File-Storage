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

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Trace(string message)
        {
            logger.Trace(message);
        }

        public void Info(string message)
        {
            logger.Info(message);
        }
    }
}
