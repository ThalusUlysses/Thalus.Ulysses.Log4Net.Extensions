namespace Thalus.Ulysses.Log4Net.Extensions.Contracts.Result
{
    public interface IError
    {
        int Code { get; }
        string Text { get; }

        bool IsException();

        Exception Exception { get; }
    }
}
