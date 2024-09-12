using System.Diagnostics;
using System.Reflection;

namespace Thalus.Ulysses.Log4Net.Extensions
{
    /// <summary>
    /// Defines the configuration for <see cref="LogEnhancer"/> as <see cref="LogEnhancerConfig"/>. Holds configuration
    /// information e.g. <see cref="Site"/> or <see cref="System"/>
    /// </summary>
    /// 
    [DebuggerDisplay("Site={Site},System={System},Machine={RunsOnMachine},App={ApplicationName},Version={ApplicationVersion}")]
    public class LogEnhancerConfig
    {
        public LogEnhancerConfig()
        {
            Site = "UNKNOWN";
            System = Environment.MachineName;
            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
            ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            RunsOnMachine = Environment.MachineName;
        }

        /// <summary>
        /// Gets or sets the Site information the <see cref="System"/> is associated with e.g BT_KA as <see cref="string"/>, default is UNKNOWN
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// Gets or sets the System information the process is running on as <see cref="string"/> e.g. KA_ORAC_UAT_TEST, default is <see cref="Environment.MachineName"/>
        /// </summary>
        public string System { get; set; }

        /// <summary>
        /// Gets or sets the application name as friendly name of the process as <see cref="string"/> e.g. CSW, XYZ_API
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the application version as friendly version of the process as <see cref="string"/> e.g 12334Ok123
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// Gets or sets the physical machine the process is running on as <see cref="string"/>, default is <see cref="Environment.MachineName"/>
        /// </summary>
        public string RunsOnMachine { get; set; }

        /// <summary>
        /// Gets or sets a collection of scrap functions. Scrap functiins scrap information from text values for e.g. pi or pii reasons
        /// </summary>
        public IEnumerable<Func<string, string>> AdditionalScrapFunctions { get; set; }
    }
}
