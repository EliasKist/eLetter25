namespace eLetter25.Application.Common.Results;

public sealed class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<Error> Errors { get; }

    private Result(bool isSuccess, IReadOnlyList<Error>? errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? [];
    }

    public static Result Success() => new(true, []);
    public static Result Failure(Error error) => new(false, [error]);
    public static Result Failure(IEnumerable<Error> errors) => new(false, errors.ToArray());

    public static Result Failure(string code, string message) =>
        new(false, [new Error(code, message)]);
}

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public IReadOnlyList<Error> Errors { get; }

    private Result(bool isSuccess, T? value, IReadOnlyList<Error>? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors ?? [];
    }

    public static Result<T> Success(T value) => new(true, value, []);
    public static Result<T> Failure(Error error) => new(false, default, [error]);
    public static Result<T> Failure(IEnumerable<Error> errors) => new(false, default, errors.ToArray());

    public static Result<T> Failure(string code, string message) =>
        new(false, default, [new Error(code, message)]);
}