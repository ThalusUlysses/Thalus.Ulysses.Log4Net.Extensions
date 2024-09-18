using Thalus.Ulysses.Log4Net.Extensions.Contracts.Result;

namespace Thalus.Ulysses.Log4Net.Extensions.Result
{
    public class Error : IError
    {
        public int Code { get; set; }

        public string Text { get; set; }

        public bool IsException()
        {
            return Exception != null;
        }

        public Exception Exception { get; set; }
    }
}
