using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Streaming.Result;

/// <summary>
/// The class that represents a generic result response.
/// </summary>
public class Result
{
    /// <summary>
    /// The constructor that initializes the result with a boolean indicating the success of the result, an error, and an HTTP status code.
    /// </summary>
    /// <param name="isSuccess">A boolean indicating the success of the result.</param>
    /// <param name="error">The error of the result.</param>
    /// <param name="statusCode">The HTTP status code associated with the result.</param>
    /// <exception cref="ArgumentException">Thrown when the want to create a successful result with an error.</exception>
    protected Result(bool isSuccess, Error? error, HttpStatusCode statusCode)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("Invalid combination: success with an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
        HttpStatusCode = statusCode;
    }

    /// <summary>
    /// The flag that indicates the failure of the result taking the negation of the success flag.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// The flag that indicates the success of the result.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// The property that indicates the error of the result e.g. NotFound, Validation, Conflict, Failure.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// The HTTP status code associated with the result.
    /// </summary>
    public HttpStatusCode HttpStatusCode { get; }

    /// <summary>
    /// Action that returns a successful result with no value and the specified HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code for the successful result (defaults to OK).</param>
    /// <returns>A successful result with the specified status code.</returns>
    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new Result(true, Error.None, statusCode);
    }

    /// <summary>
    /// The action that returns a successful result with a value and the specified HTTP status code.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    /// <param name="statusCode">The HTTP status code for the successful result (defaults to OK).</param>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>A successful result with a value and the specified status code.</returns>
    public static Result<TValue> Success<TValue>(
        TValue value,
        HttpStatusCode statusCode = HttpStatusCode.OK
    )
    {
        return new Result<TValue>(value, true, Error.None, statusCode);
    }

    /// <summary>
    /// The action that returns a failure result with an error and the specified HTTP status code.
    /// </summary>
    /// <param name="error">The error of the result.</param>
    /// <param name="statusCode">The HTTP status code for the failure result.</param>
    /// <returns>A failure result with an error and the specified status code.</returns>
    public static Result Failure(Error? error, HttpStatusCode statusCode)
    {
        return new Result(false, error, statusCode);
    }

    /// <summary>
    /// The action that returns a failure result with an error and the specified HTTP status code.
    /// </summary>
    /// <param name="error">The error of the result.</param>
    /// <param name="statusCode">The HTTP status code for the failure result.</param>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>A failure result with an error and the specified status code.</returns>
    public static Result<TValue> Failure<TValue>(Error? error, HttpStatusCode statusCode)
    {
        return new Result<TValue>(default, false, error, statusCode);
    }
}

/// <summary>
/// The class that represents a generic result response with a value.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public class Result<TValue> : Result
{
    /// <summary>
    /// The value of the result.
    /// </summary>
    private readonly TValue? _value;

    /// <summary>
    /// The constructor that initializes the result with a value, a boolean indicating the success of the result, an error, and an HTTP status code.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    /// <param name="isSuccess">A boolean indicating the success of the result.</param>
    /// <param name="error">The error of the result.</param>
    /// <param name="statusCode">The HTTP status code associated with the result.</param>
    protected internal Result(
        TValue? value,
        bool isSuccess,
        Error? error,
        HttpStatusCode statusCode
    )
        : base(isSuccess, error, statusCode)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                "The value of a failure result can't be accessed."
            );

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null
            ? Success(value)
            : Failure<TValue>(Error.NullValue, HttpStatusCode.BadRequest);

    public static Result<TValue> ValidationFailure(Error? error) =>
        new(default, false, error, HttpStatusCode.BadRequest);
}
