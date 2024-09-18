namespace Thalus.Ulysses.Log4Net.Extensions
{
    public class RemoteLogIngestionExampleConfig    
    { 
        /// <summary>
        /// Gets or sets a rule id that might be applied to your log ingestion
        /// using a rule ID for data collection is a very common thing
        /// in dentifying in which bucket the logs are piped in
        /// </summary>
        public string RuleId { get; set; }

        /// <summary>
        /// Gets or sets the data stream id that might be applied to process the 
        /// log after ingestion. This is very common in backend logging frameworks
        /// to direct the processing through specified streams
        /// </summary>
        public string DataStreamId { get; set; }

        /// <summary>
        /// Gets or sets the connection string of the backend you are ingesting data
        /// Mainly this s a http(s) connection or something that is based
        /// on the mechanics of http(s)
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the ingestion intervall of the entries. This setting is used
        /// to create batch requests instead of individually push each request 
        /// to the backend. Together with <see cref="MaxEntries"/> and <see cref="MaxConcurrentRequests"/>
        /// you are abloe to create a payload to requests ratio that fits you needs
        /// </summary>
        public TimeSpan IngestionIntervall { get; set; }

        /// <summary>
        /// Gets or sets the max entries to be chached locally before batching
        /// </summary>
        public int MaxEntries { get; set; }

        /// <summary>
        /// Gets or sets the Max concurrent requests to teh backend. This is needed
        /// together with <see cref="IngestionIntervall"/> to create a payload
        /// to requets ratio that best fits your needs
        /// </summary>
        public int MaxConcurrentRequests { get; set; }
    }
}
