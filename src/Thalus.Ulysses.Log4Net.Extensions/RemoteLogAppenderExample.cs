using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Ingestion;
using log4net.Appender;
using log4net.Core;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Thalus.Ulysses.Log4Net.Extensions
{
    /// <summary>
    /// This is the my appender, see it as a recommendation how to use it
    /// YOu can do any type of magic in the <see cref="Append(LoggingEvent)"/> method
    /// like using a connector to any http based log sink. Key component is
    /// the LoggingEnhancer which does all the leg work for you. Examplelog4net.config is
    /// a brief example of how to configure the appender and / or the enhancer.
    /// HAVE FUN!
    /// </summary>
    public class RemoteLogAppenderExample : AppenderSkeleton
    {
        LogEnhancer _enhancer;
        LogEnhancerConfig _config = new LogEnhancerConfig();
        object _lockEnhancerCreation = new object();

        RemoteLogIngestionExample _logIngestion;
   
        public RemoteLogAppenderExample()
        {

        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            // This makes all properties of LoggingEvent properly set
            // see https://logging.apache.org/log4net/log4net-1.2.15/release/sdk/html/P_log4net_Core_LoggingEvent_Fix.htm
            loggingEvent.Fix = FixFlags.All;

            // ensure that the enhacer is created just once
            lock (_lockEnhancerCreation)
            {
                if (_enhancer == null)
                {
                    _enhancer = new LogEnhancer(_config);
                }

                if (_logIngestion == null)
                {
                    var config = new RemoteLogIngestionExampleConfig
                    { 
                        DataStreamId = DataStreamId,
                        RuleId = RuleId, 
                        ConnectionString = ConnectionString,
                        IngestionIntervall = IngestionIntervall
                    };

                    _logIngestion = new RemoteLogIngestionExample(config);
                }              
            }

            var item = _enhancer.Enhance(loggingEvent);

           var result = _logIngestion.IngestAsync(item).Result;
        }

        public string Site { get => _config.Site; set => _config.Site = value; }
        public string System { get => _config.System; set => _config.System = value; }
        public string ApplicationName { get => _config.ApplicationName; set => _config.ApplicationName = value; }
        public string ApplicationVersion { get => _config.ApplicationVersion; set => _config.ApplicationVersion = value; }
        public string MachineName { get => _config.RunsOnMachine; set => _config.RunsOnMachine = value; }
        public string ConnectionString { get; set; }
        public string RuleId { get; set; }
        public string DataStreamId { get; set; }

        public TimeSpan IngestionIntervall { get; set; }
    }
}
