using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thalus.Ulysses.Log4Net.Extensions.Contracts.Trace;

namespace Thalus.Ulysses.Log4Net.Extensions.Trace
{
    [DebuggerDisplay("{CategoryText} {CallerMember} {Text}")]
    public class TraceEntryDTO : ITraceEntry
    {
        public int ThreadId { get; set; }

        public string CategoryText { get; set; }

        public int CategoryInt { get; set; }

        public string Text { get; set; }

        public object Data { get; set; }

        [DebuggerDisplay("{Utc} {Local}")]
        public DateTime Utc { get; set; }

        [DebuggerDisplay("{Utc} {Local}")]
        public DateTime Local { get; set; }

        [DebuggerDisplay("{FileName} {CallerMember} {Line}")]
        public int Line { get; set; }

        public string Scope { get; set; }

        [DebuggerDisplay("{FileName} {CallerMember} {Line}")]
        public string FileName { get; set; }
        
        [DebuggerDisplay("{FileName} {CallerMember} {Line}")]
        public string CallerMember { get; set; }

        public IEnumerable<string> Attributes { get; set; }

        public string System { get; set; }

        [DebuggerDisplay("{ApplicationName} {ApplicationVersion}")]

        public string ApplicationName { get; set; }

        [DebuggerDisplay("{ApplicationName} {ApplicationVersion}")]
        public string ApplicationVersion { get; set; }

        public string Site { get; set; }

        public int ProcessId { get; set; }

        public string ProcessName { get; set; }

        public IEnumerable<KeyValuePair<string, object>> KVPairs { get; set; }

        public string User { get; set; }

        public string ISORegionName { get; set; }
    }
}
