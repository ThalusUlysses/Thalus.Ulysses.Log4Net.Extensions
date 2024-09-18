using Thalus.Ulysses.Log4Net.Extensions.Contracts.Result;

namespace Thalus.Ulysses.Log4Net.Extensions.Result
{
    public class Result<TType> : IResult<TType>
    {
        public static IResult<TDataType> Ok<TDataType>(string text = null, int code = 200, TDataType data = default)
        {
            return new Result<TDataType>() { Code = code, Text = text, Data = data };

        }

        public static IResult<TDataType> Error<TDataType>(string text, int code = 400, TDataType data = default)
        {
            return new Result<TDataType>() { Code = code, Text = text, Data = data };

        }

        public static IResult<TDataType> Error<TDataType>(string text, int code = 400, params Error[] errors)
        {
            return new Result<TDataType>() { Code = code, Text = text, Errors = errors };

        }

        public static IResult<TDataType> Fatal<TDataType>(Exception ex,string text, int code = 500)
        {
            return new Result<TDataType>() { Code = code, Text = text, Errors = new[] { new Error { Code = 500, Exception = ex, Text = ex.Message } } };

        }

        public int Code { get; set; }

        public string Text { get; set; }

        public TType Data { get; set; }

        public IEnumerable<Error> Errors { get; set; }

        public TType GetData()
        {
          return Data;
        }

        public TType GetDataAs<TDataType>() where TDataType : TType
        {
            return (TDataType)Data;
        }

        public IEnumerable<IError> GetErrors()
        {
            return Errors.Cast<IError>();
        }

        public bool HasErrors()
        {
            return Errors != null && Errors.Any();
        }

        public bool IsSuccessful()
        {
            return Code >= 200 && Code < 400;
        }

    }
}
