using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Ingestion;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Thalus.Ulysses.Log4Net.Extensions.Contracts.Result;
using Thalus.Ulysses.Log4Net.Extensions.Contracts.Trace;
using Thalus.Ulysses.Log4Net.Extensions.Result;

namespace Thalus.Ulysses.Log4Net.Extensions
{
    public class RemoteLogIngestionExample
    {
        RemoteLogIngestionExampleConfig _config;
        object _rWLockTraceEntries = new object();
        List<ITraceEntry> _entries = new List<ITraceEntry>();
        DateTime _storeLastSentDateTimeUtc;

        /// <summary>
        /// Sets the Maximum allowed concurrent API / Backend requests.
        /// This is needed to get a good payload tor requests ratio
        /// </summary>
        const int MAX_CONCURRENCY = 10;

        /// <summary>
        /// Sets the Maximum number of trace entries stored / cached
        /// Once the max is reached the oldest entriy is scapped
        /// Together with the <see cref="MAX_CONCURRENCY"/> cou
        /// can create a payload to requests ratio that fits
        /// </summary>
        const int MAX_STORED_ENTRIES = 100;

        static TimeSpan DEFAULT_INGESTION_INTERVALL = new TimeSpan(0, 5, 0);

        /// <summary>
        /// Creates an instance of <see cref="RemoteLogIngestionExample"/> with the
        /// passed parameters
        /// 
        /// This implementation shows how a modern ingestion could work with a 
        /// backend.
        /// </summary>
        /// <param name="config"></param>
        public RemoteLogIngestionExample(RemoteLogIngestionExampleConfig config)
        {
            CreateIngestionClient(config);
        }

        private void CreateIngestionClient(RemoteLogIngestionExampleConfig config)
        {
            if (config == null)
            {
                _config = DefaultConfig();
                return;
            }

            try
            {
                // TODO
                // Do your ingestion client creation here e.g. like use RestSharp 
                // or use something out of the azure Continuum and so forth
                //.
                //.
                //.
                _config = config;
            }
            catch (Exception)
            {
                _config = DefaultConfig();
            }
        }


        private void Store(ITraceEntry entry)
        {   
            lock (_entries)
            {
                Console.WriteLine(JsonConvert.SerializeObject(entry, Formatting.Indented));

                if (_entries.Count > _config.MaxEntries + 1)
                {
                    _entries.RemoveAt(0);
                }
                _entries.Add(entry);
            }
        }

        RemoteLogIngestionExampleConfig DefaultConfig()
        {
            return new RemoteLogIngestionExampleConfig 
            {
                MaxConcurrentRequests = MAX_CONCURRENCY, 
                IngestionIntervall = DEFAULT_INGESTION_INTERVALL,
                MaxEntries = MAX_STORED_ENTRIES
            };
        }

        private async Task<IResult<object>> PushStored()
        {
            var entries = SafeDequeueEntries();

            if (!CanIngest())
            {
                return Result<object>.Error<object>($"Could not ingest data, prerequisites not fulfilled see={nameof(CanIngest)}");
            }

            return await DoIngest(entries);
        }

        bool CanIngest()
        {
            // TODO
            // implement your predicate for ingestion here
            // e.g. check if your client is created
            // or your have access to your backend
            //.
            //.
            //.
            return false;
        }

        private async Task<IResult<object>> DoIngest(IEnumerable<ITraceEntry> entries)
        {
            // TODO
            // Do all the ingestion work here
            // transform the trace entries into your entities
            // and ingest it to your backend
            //.
            //.
            //.
            throw new NotImplementedException("Implement your log ingestion here");            
        }

        private IEnumerable<ITraceEntry> SafeDequeueEntries()
        {
            ITraceEntry[] temp;
            lock (_rWLockTraceEntries)
            {
                temp = _entries.ToArray();
                _entries = new List<ITraceEntry>();
            }

            return temp;
        }       


        public async Task<IResult<object>> IngestAsync(ITraceEntry trace)        
        {
            try
            {
                Store(trace);

                // this is for batching, as we do not want to push each message individually
                // we like to limit the access to a defined intervall
                // this leads to non real time apporach but helps keeping
                // load on API low. Most of the logging backends work that way
                // preffereing bigger payload over high frequent requests
                // together with the MAX_ENTRIES setting you can create a
                // fitting leveraged payload to requests ratio

                var utcNow = DateTime.UtcNow;

                if (!IngestionIntervallElapsed(utcNow))
                {
                    return Result<object>.Ok<object>($"Nothing to ingest");
                }

               var result = await PushStored();

                _storeLastSentDateTimeUtc = utcNow;

                return result;
            }
            catch (Exception ex)
            {
                return Result<object>.Fatal<object>(ex, $"Ingestion of data failed fatally");
            }
        }

        bool IngestionIntervallElapsed(DateTime utcNow)
        {
            return (utcNow - _storeLastSentDateTimeUtc < _config.IngestionIntervall);
        }
    }
}
