using System.Reflection;

namespace Domain.Primitives;

public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, null);
    public static Result<T> Failure<T>(Error error) => new(default, false, error);

    public static TResponse FailureTyped<TResponse>(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Failure(error);

        var valueType = typeof(TResponse).GetGenericArguments()[0];
        var method = typeof(Result).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Failure" && m.IsGenericMethod && m.GetParameters().Length == 1)
            .MakeGenericMethod(valueType);
        return (TResponse)method.Invoke(null, [error])!;
    }
}

public sealed class Result<T> : Result
{
    private readonly T? _value;

    internal Result(T? value, bool isSuccess, Error? error) : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");
}
