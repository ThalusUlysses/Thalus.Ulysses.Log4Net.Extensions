namespace Thalus.Ulysses.Log4Net.Extensions.Contracts.Trace
{
    public interface ITraceEntry
    {
        /// <summary>
        /// Gets the thread id of the component the log entry was triggered from
        /// </summary>
        int ThreadId { get; }

        /// <summary>
        /// Gets the log level category as <see cref="string"/> out of <see cref="TraceCategory"/> e.g <see cref="TraceCategory.Error"/>
        /// or <see cref="TraceCategory.Fatal"/>
        /// </summary>
        string CategoryText { get; }

        /// <summary>
        /// Gets the log level representation as <see cref="int"/> corresponding to the <see cref="string"/> value in <see cref="CategoryText"/>
        /// </summary>
        int CategoryInt { get; }

        /// <summary>
        /// Gets the log message text as <see cref="string"/>. This is a free text field, any string is accepted
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Gets the system name where the process is running 
        /// that produced the log message. This is usually configurable in a configuration file. Default is <see cref="Environment.MachineName"/>
        /// </summary>
        string System { get; }

        /// <summary>
        /// Gets the site the <see cref="System"/> is associated with. This is usually configurable in a configuration file. Default is UNKNOWN
        /// </summary>
        string Site { get; }

        /// <summary>
        /// Gets the raw data of a log entry. Value is <see cref="null"/> if no data has been passed. If the data that has been passed is serializable
        /// the value remains null and the data is being stored in <see cref="KVPairs"/>
        /// </summary>
        object Data { get; }

        /// <summary>
        /// Gets the timestamp of the log entry as UTC <see cref="DateTime"/>
        /// </summary>
        DateTime Utc { get; }

        /// <summary>
        /// Gets the timestamp of the log entry as local <see cref="DateTime"/>
        /// </summary>
        DateTime Local { get; }

        /// <summary>
        /// Gets the line number which raised the log entry as <see cref="int"/>. If the line number is not extractable from the tracing, default is -1
        /// </summary>
        int Line { get; }

        /// <summary>
        /// Gets the process id of the process that has raised the log entry as <see cref="int"/> derived from <see cref="Process.GetCurrentProcess()"/>
        /// </summary>
        int ProcessId { get; }

        /// <summary>
        /// Gets the process name of the process that has raised the log enrty as <see cref="string"/> derived from <see cref="Process.GetCurrentProcess().ProcessName"/>
        /// </summary>
        /// <remarks>
        /// Please note that the <see cref="ProcessName"/> and <see cref="ApplicationName"/> can be different. It might be that the process is named or registered 
        /// differently from the user friendly name of the application.
        /// </remarks>
        string ProcessName { get; }

        /// <summary>
        /// Gets the application name of the appliaction that has raised the log entry as <see cref="string"/>. 
        /// This value is usually configurable and marks the applciations friendly name
        /// </summary>
        ///Please note that the <see cref="ProcessName"/> and <see cref="ApplicationName"/> can be different. It might be that the process is named or registered 
        /// differently from the user friendly name of the application.
        /// </remarks>
        string ApplicationName { get; }

        /// <summary>
        /// Gets the application version of the application that has raised the log entry as <see cref="string"/>.
        /// This value is usually configurable and marks the applications user friendly version string
        /// </summary>
        string ApplicationVersion { get; }

        /// <summary>
        /// Gets the scope of that was chosen for the log context when the log entry has been raised as <see cref="string"/>.
        /// This is something that usually comes from the logging framework, if not set DEFAULT is used
        /// </summary>
        string Scope { get; }

        /// <summary>
        /// Gets the file name in which the log entry has been raised as <see cref="string"/>. If no location information is available
        /// this value is set to NOTSET
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Gets the Method or caller name in which the log entry has been raised as <see cref="string"/>. If no location information is available
        /// this value is set to NOTSET
        /// </summary>
        string CallerMember { get; }

        /// <summary>
        /// Gets a collection of attributes that have been associated with the log entry. A unsorted list of tags as <see cref="IEnumerable{string}"/>
        /// </summary>
        IEnumerable<string> Attributes { get; }

        /// <summary>
        /// Gets a collection of Key/Value pairs that have been added to the log entry. These are maninly items that come form serializable values in <see cref="Data"/> property but
        /// not limited
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> KVPairs { get; }

        /// <summary>
        /// Gets the user id as scrapped string. You may be able to identify the user by token but not in clear text
        /// </summary>
        string User { get; }

        /// <summary>
        /// Gest the ISO region name three letter code
        /// </summary>
        string ISORegionName { get; }
    }
}
