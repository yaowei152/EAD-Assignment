/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

namespace ArcaneVault.Web.Services;

/// <summary>
/// Wraps the outcome of a call to the Web API so Razor Page handlers can branch on
/// success/failure and copy field-level validation errors straight into their own ModelState.
/// </summary>
public class ApiResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public int StatusCode { get; init; }
    public string? ErrorTitle { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    public static ApiResult<T> Ok(T data, int statusCode = 200) =>
        new() { Success = true, Data = data, StatusCode = statusCode };

    public static ApiResult<T> Fail(int statusCode, string? title, Dictionary<string, string[]>? errors = null) =>
        new() { Success = false, StatusCode = statusCode, ErrorTitle = title, ValidationErrors = errors };
}
