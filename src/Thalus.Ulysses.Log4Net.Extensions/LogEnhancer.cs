using log4net.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Thalus.Ulysses.Log4Net.Extensions.Contracts.Trace;
using Thalus.Ulysses.Log4Net.Extensions.Trace;

namespace Thalus.Ulysses.Log4Net.Extensions
{

    public class LogEnhancer
    {
        const string NOT_SET_STRING = "NOTSET";
        const int NOT_SET_INT = -1;

        LogEnhancerConfig _config;

        List<Func<string, string>> _stringOperations;

        /// <summary>
        /// Initializes an instance of <see cref="LogEnhancer"/> with the passed configuration
        /// object
        /// </summary>
        /// <param name="config">Pass configuration object</param>
        /// <exception cref="ArgumentNullException"></exception>
        public LogEnhancer(LogEnhancerConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config), $"Passed parameter={nameof(config)} with type={typeof(LogEnhancerConfig).Name} MUST not be null");
            }

            _config = config;

            _stringOperations = new List<Func<string, string>>
            {
                ScrapIpAdress,
                ScrapPassword,
                ScrapUser,
                ScrapUrl
            };

            if (_config.AdditionalScrapFunctions != null)
            {
                _stringOperations.AddRange(_config.AdditionalScrapFunctions);
            }
        }

        /// <summary>
        /// Tries to add eception data as key/value pair ot the dictionary.
        /// </summary>
        /// <param name="kvPairs"></param>
        /// <param name="ex"></param>
        /// <returns>Returns true if the conversion was perfromed successfully, otherwise false</returns>
        /// <remarks>It can happen, that an exception is not serializeable and therefore not able to be added as KV</remarks>
        private bool AddExceptionDataTo(Dictionary<string, object> kvPairs, Exception ex)
        {
            if (ex != null)
            {
                var exceptionKV = CreateFromData(ex);
                if (exceptionKV == null)
                {
                    return false;
                }
                foreach (var kv in exceptionKV)
                {
                    kvPairs[kv.Key] = kv.Value;
                }
            }

            return true;
        }

        /// <summary>
        /// Extracts thread ID from <see cref="LoggingEvent.ThreadName"/>. This is usually the <see cref="Environment.CurrentManagedThreadId"/>
        /// </summary>
        /// <param name="e"></param>
        /// <returns>returns an integer representation of the <see cref="LoggingEvent.ThreadName"/> if convertible, otherwise <see cref="NOT_SET_INT"/></returns>
        private int ExtractThreadId(LoggingEvent e)
        {
            int threadId;
            if (!int.TryParse(e.ThreadName, out threadId))
            {
                threadId = NOT_SET_INT;
            }

            return threadId;
        }

        private void AddCommonAttributesTo(List<string> attributes)
        {
            
        }

        /// <summary>
        /// Adds common data to the key value pairs e.g. ExecutingAssembly and so forth
        /// </summary>
        /// <param name="kvPairs"></param>
        /// <param name="e"></param>
        private void AddCommonDataTo(Dictionary<string, object> kvPairs, LoggingEvent e)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var r = RegionInfo.CurrentRegion;
            var logging = Assembly.GetAssembly(typeof(LoggingEvent)).GetName();
            kvPairs["ExecutingAssemblyName"] = executingAssembly.GetName().Name;
            kvPairs["ExecutingAssemblyVersion"] = executingAssembly.GetName().Version.ToString();
            kvPairs["AppDomainFriendlyName"] = AppDomain.CurrentDomain.FriendlyName;
            kvPairs["RunsOnMachine"] = _config.RunsOnMachine;
            kvPairs["OriginalLoggingFramework"] = $"{logging.Name}:{logging.Version}";
            kvPairs["OriginalLogLevelText"] = e.Level.Name;
            kvPairs["OriginalLogLevelInt"] = e.Level.Value;
            kvPairs["RegionGeoID"] = r.GeoId;
            kvPairs["RegionNativeName"] = r.NativeName;
            kvPairs["RegionEnglishName"] = r.EnglishName;

        }

        /// <summary>
        /// Enhances a trace entry by several parameters and attributes extracted from the
        /// original <see cref="LoggingEvent"/> provided by log4Net
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Returns an entry of type <see cref="ITraceEntry"/> that contanins structured data as good as possible</returns>
        public ITraceEntry Enhance(LoggingEvent e)
        {
            Dictionary<string, object> kvPairs = new Dictionary<string, object>();
            List<string> attributes = new List<string>();

            var eventData = e.GetLoggingEventData();
            var locationInformation = e.LocationInformation;
            
            var category = MapCategory(e.Level);
            var threadId = ExtractThreadId(e);
            
            var convertToKvOk = AddExceptionDataTo(kvPairs, e.ExceptionObject);

            AddCommonDataTo(kvPairs, e);

            var r = RegionInfo.CurrentRegion;
            
            AddCommonAttributesTo(attributes);

            var traceObject = new TraceEntryDTO
            {
                CategoryInt = (int)category,
                CategoryText = Enum.GetName(category),
                Local = e.TimeStamp,
                Utc = e.TimeStampUtc,
                ProcessId = Environment.ProcessId,
                ProcessName = Process.GetProcessById(Environment.ProcessId).ProcessName,
                ApplicationName = _config.ApplicationName,
                ApplicationVersion = _config.ApplicationVersion,
                Site = _config.Site,
                System = _config.System,
                Text = e.RenderedMessage,
                Scope = e.LoggerName,
                ThreadId = threadId,
                Attributes = attributes.Any() ? attributes : null,
                KVPairs = kvPairs.Any() ? kvPairs : null,
                Data = convertToKvOk ? null : e.ExceptionObject,
                User = Environment.UserDomainName != null ? GetHashString(Environment.UserDomainName) : GetHashString(Environment.UserName),
                ISORegionName = r.ThreeLetterISORegionName
            };

            AdjustLocationInfo(locationInformation, traceObject);

            return AdjustAndScrapText(traceObject);
        }

        private void AdjustLocationInfo(LocationInfo locationInformation, TraceEntryDTO traceObject)
        {
            if (locationInformation != null)
            {
                traceObject.CallerMember = locationInformation?.MethodName;
                traceObject.Line = int.Parse(locationInformation?.LineNumber);
                traceObject.FileName = locationInformation?.FileName;
            }
            else
            {
                traceObject.CallerMember = NOT_SET_STRING;
                traceObject.Line = NOT_SET_INT;
                traceObject.FileName = NOT_SET_STRING;
            }
        }

        /// <summary>
        /// Deserializes and flattens a json string representation into a flat list
        /// of key value pairs using the parent property as preceding identifier
        /// </summary>
        /// <param name="json">Pass a json string</param>
        /// <returns>Returns a flat Dictionary</returns>
        private Dictionary<string, object> DeserializeAndFlatten(string json)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            JToken token = JToken.Parse(json);
            FillDictionaryFromJToken(dict, token, "");
            return dict;
        }

        private void FillDictionaryFromJToken(Dictionary<string, object> dict, JToken token, string prefix)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        FillDictionaryFromJToken(dict, prop.Value, Join(prefix, prop.Name));
                    }
                    break;

                case JTokenType.Array:
                    int index = 0;
                    foreach (JToken value in token.Children())
                    {
                        FillDictionaryFromJToken(dict, value, Join(prefix, index.ToString()));
                        index++;
                    }
                    break;

                default:
                    dict.Add(prefix, ((JValue)token).Value);
                    break;
            }
        }

        /// <summary>
        /// Joins parent property and property name using . e.g. parent.name
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string Join(string prefix, string name)
        {
            return (string.IsNullOrEmpty(prefix) ? name : prefix + "." + name);
        }

     
        /// <summary>
        /// Uses RegEx to find any IP adrees in a given string and replace it by an anonymized
        /// strring pattern.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks>You will be able to have the same representation per IP address but not know which address it was in particular.
        /// E.g 192.168.0.1 will always result in -> SHD9786SD89</remarks>
        string ScrapIpAdress(string text)
        {
            var match = Regex.Match(text, "(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})");

            if (match.Success)
            {
                string impersonatedIP = GetHashString(match.Value);
                text = text.Replace(match.Value, $"SCRAPPED_IP{impersonatedIP}");
            }

            return text;
        }

        /// <summary>
        /// Uses string contains patterns to find passwords in text e.g. like "password" or "pw" and replace it by an anonymized string pattern
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string ScrapPassword(string text)
        {
            var patterns = new[] { "password", "pw" };
           
            string workingText = text;
           
            foreach (var pattern in patterns) 
            {
                workingText = ScrapText(workingText, pattern);
            }

            return workingText;
        }

        /// <summary>
        /// Uses string contains patterns to find user names in text e.g. like "user" or "usr" and replace it by an anonymized string pattern
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string ScrapUser(string text)
        {
            var patterns = new[] { "user", "usr" };

            string workingText = text;

            foreach (var pattern in patterns)
            {
                workingText = ScrapText(workingText, pattern);
            }

            return workingText;
        }

        /// <summary>
        /// Uses regex to fin all occurences of an URL in a text and replaces them with an anonymized stirng pattern
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string ScrapUrl(string text)
        {
            var match = Regex.Matches(text, "[(http(s)?):\\/\\/(www\\.)?a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)");

            foreach (var item in match)
            {
                Match m = (Match)item;
                text = text.Replace(m.Value, $"SCRAPPED_URL{GetHashString(m.Value)}");
            }           

            return text;
        }

        /// <summary>
        /// Scraps a text from a given text by replcing it with an anonymized pattern. The limit is set to 32 characters
        /// which means for text that are longer than 32 characters the rest will be unscrapped
        /// </summary>
        /// <param name="text"></param>
        /// <param name="scrapText"></param>
        /// <returns></returns>
        string ScrapText(string text,string scrapText)
        {
            string workingText = text.ToLowerInvariant();

            var foundIndexes = new List<int>();

            for (int i = workingText.IndexOf(scrapText); i > -1; i = workingText.IndexOf(scrapText, i + 1))
            {
                // for loop end when i=-1 ('a' not found)
                foundIndexes.Add(i);
            }
            if (!foundIndexes.Any())
            {
                return text;
            }

            StringBuilder b = new StringBuilder();

            b.Append(text.Substring(0, foundIndexes.First()));
            foreach (var index in foundIndexes) 
            {
                b.Append(text.Substring(index, scrapText.Length));
                if (text.Length > (index + scrapText.Length + 32))
                {
                    b.Append(GetHashString(text.Substring(index + scrapText.Length, 32)));
                }
                else
                {
                    b.Append(GetHashString(text.Substring(index + scrapText.Length, text.Length - index - scrapText.Length)));
                }
            }
            var lastIdx = foundIndexes.Last(); 
            if (text.Length > (lastIdx + scrapText.Length + 32))
            {
                b.Append(text.Substring(foundIndexes.Last() + scrapText.Length + 32));
            }

            return b.ToString();
        }        

        /// <summary>
        /// Adjusts the text of the trace entry and checks for text patterns like ERROR or ORACLE and so forth
        /// </summary>
        /// <param name="trace"></param>
        /// <returns></returns>
        TraceEntryDTO AdjustAndScrapText(TraceEntryDTO trace)
        {
            Dictionary<string, object> additionalKVps = new Dictionary<string, object>();

            if (trace.Text.Contains("Error", StringComparison.InvariantCultureIgnoreCase))
            {
                additionalKVps["InTextErrorIndication"] = true;
            }

            if (trace.Text.Contains("fatal", StringComparison.InvariantCultureIgnoreCase))
            {
                additionalKVps["InTextFatalIndication"] = true;
            }

            if (trace.Text.Contains("ORA-", StringComparison.InvariantCultureIgnoreCase))
            {
                additionalKVps["InTextOracleErrorIndication"] = true;
            }

            if (trace.Text.Contains("ORACLE", StringComparison.InvariantCultureIgnoreCase))
            {
                additionalKVps["InTextOracleIndication"] = true;
            }

            foreach (var item in _stringOperations)
            {
                trace.Text = item(trace.Text);
            }

            if (trace.KVPairs != null)
            {
                foreach (var item in trace.KVPairs)
                {
                    additionalKVps[item.Key] = item.Value;
                }
                trace.KVPairs = additionalKVps;
            }
            else
            {
                if (additionalKVps.Any())
                {
                    trace.KVPairs = additionalKVps;
                }
            }           

            return trace;

        }

        public byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString().Substring(0, 16);
        }

        Dictionary<string, object> CreateFromData(object data)
        {
            try
            {
                if (data == null)
                {
                    return null;
                }

                Dictionary<string, object> kvp = null;
                if (data is string)
                {
                    var str = (string)data;

                    //Most likly a json alread
                    if (str.StartsWith("{") && str.EndsWith("}"))
                    {
                        kvp = DeserializeAndFlatten(str);
                    }
                }
                else
                {
                    var jsonData = JsonConvert.SerializeObject(data);
                    kvp = DeserializeAndFlatten(jsonData);
                }

                return kvp;

            }
            catch (Exception)
            {
                return null;
            }
        }

        Dictionary<string, TraceCategory> _categoryMap = new Dictionary<string, TraceCategory>
        {
            { Level.Error.Name, TraceCategory.Error},
            { Level.Info.Name, TraceCategory.Info},
            { Level.Warn.Name, TraceCategory.Warning},
            { Level.Debug.Name, TraceCategory.Debug},
            { Level.Fatal.Name, TraceCategory.Fatal},
            { Level.Off.Name, TraceCategory.Debug},
            { Level.Alert.Name, TraceCategory.Fatal},
            { Level.All.Name, TraceCategory.Fatal},
            { Level.Critical.Name, TraceCategory.Fatal},
            { Level.Emergency.Name, TraceCategory.Fatal},
            { Level.Fine.Name, TraceCategory.Unnkown},
            { Level.Finer.Name, TraceCategory.Unnkown},
            { Level.Finest.Name, TraceCategory.Unnkown},
            { Level.Notice.Name, TraceCategory.Unnkown},
            { Level.Severe.Name, TraceCategory.Unnkown},
            { Level.Verbose.Name, TraceCategory.Unnkown},
            { Level.Trace.Name, TraceCategory.Unnkown},
        };
        TraceCategory MapCategory(Level level)
        {
            TraceCategory category;
            if (!_categoryMap.TryGetValue(level.Name, out category))
            {
                return TraceCategory.Debug;
            }

            return category;
        }
    }
}
