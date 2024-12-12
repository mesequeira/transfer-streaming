namespace Streaming.Result;

/// <summary>
/// The error that caused the validation to fail.
/// </summary>
public sealed record ValidationError(Error?[] Errors)
    : Error("Validation.General", "One or more validation errors occurred", ErrorType.Validation)
{
    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}
