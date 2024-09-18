namespace Thalus.Ulysses.Log4Net.Extensions.Contracts.Result
{
    public interface IResult<TType>
    {
        int Code { get; }
        bool IsSuccessful();

        string Text { get; }

        bool HasErrors();

        IEnumerable<IError> GetErrors();

        TType GetData();

        TType GetDataAs<TDataType>() where TDataType : TType;
    }
}
