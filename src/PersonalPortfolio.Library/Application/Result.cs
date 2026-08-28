namespace PersonalPortfolio.Library.Application;

public enum HttpStatusResult
{
    NotApplicable,
    SuccessfulRequestWithContent,
    SuccessfulRequestWithNoContent,
    BadRequest,
    TooManyRequest,
    ServerError,
    Unauthorized,
    PreconditionFailed,
    NotFound
}

public class Result<T>
{
    private Result(T data, bool isValid, List<string> errors = null, HttpStatusResult httpResult = HttpStatusResult.NotApplicable)
    {
        Data = data;
        IsSuccessful = isValid;
        Errors = errors ?? [];
        HttpResult = httpResult;
    }

    public T Data { get; }
    public bool IsSuccessful { get; }
    public bool IsNotSuccessful => IsSuccessful is false;
    public List<string> Errors { get; }
    public HttpStatusResult HttpResult { get; }

    public static Result<T> NotSuccessful(List<string> errors = null, HttpStatusResult httpResult = HttpStatusResult.NotApplicable)
    {
        return new Result<T>(default, false, errors, httpResult);
    }

    public static Result<T> Successful(T data, HttpStatusResult httpStatusResult = HttpStatusResult.NotApplicable)
    {
        return new Result<T>(data, true, null, httpStatusResult);
    }
}