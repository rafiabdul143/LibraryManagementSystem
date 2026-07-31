
namespace LibraryManagement.Web.Services;

/// <summary>
/// Standard success/failure envelope returned by service methods that
/// perform writes, so controllers can surface validation/business-rule
/// failures (e.g. "No available copies") without throwing exceptions
/// for expected, user-facing conditions.
/// </summary>
public class ServiceResult
{
    public bool Succeeded { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ServiceResult Success() => new() { Succeeded = true };

    public static ServiceResult Failure(params string[] errors) =>
        new() { Succeeded = false, Errors = errors.ToList() };
}

/// <summary>Same as ServiceResult but carries a return payload on success.</summary>
public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Success(T data) =>
        new() { Succeeded = true, Data = data };

    public static new ServiceResult<T> Failure(params string[] errors) =>
        new() { Succeeded = false, Errors = errors.ToList() };
}