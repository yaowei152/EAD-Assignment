/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ArcaneVault.Shared.Dtos;
using ArcaneVault.Shared.Dtos.Analytics;
using ArcaneVault.Shared.Dtos.Auth;

namespace ArcaneVault.Web.Services;

/// <summary>
/// Typed HttpClient wrapper - the only place in the Web project that talks to ArcaneVault.Api.
/// Every Razor Page handler goes through this client rather than touching the database directly.
/// </summary>
public class ArcaneVaultApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public ArcaneVaultApiClient(HttpClient http)
    {
        _http = http;
    }

    // ----- Auth -----

    public Task<ApiResult<UserDto>> RegisterAsync(RegisterRequestDto dto) =>
        SendAsync<RegisterRequestDto, UserDto>(HttpMethod.Post, "api/auth/register", dto);

    public Task<ApiResult<UserDto>> LoginAsync(LoginRequestDto dto) =>
        SendAsync<LoginRequestDto, UserDto>(HttpMethod.Post, "api/auth/login", dto);

    // ----- Categories -----

    public Task<ApiResult<List<CategoryDto>>> GetCategoriesAsync() =>
        SendAsync<List<CategoryDto>>(HttpMethod.Get, "api/categories");

    public Task<ApiResult<CategoryDto>> GetCategoryAsync(string code) =>
        SendAsync<CategoryDto>(HttpMethod.Get, $"api/categories/{Uri.EscapeDataString(code)}");

    public Task<ApiResult<CategoryDto>> CreateCategoryAsync(CategoryDto dto) =>
        SendAsync<CategoryDto, CategoryDto>(HttpMethod.Post, "api/categories", dto);

    public Task<ApiResult<CategoryDto>> UpdateCategoryAsync(string code, CategoryDto dto) =>
        SendAsync<CategoryDto, CategoryDto>(HttpMethod.Put, $"api/categories/{Uri.EscapeDataString(code)}", dto);

    public Task<ApiResult<object?>> DeleteCategoryAsync(string code) =>
        SendAsync<object?>(HttpMethod.Delete, $"api/categories/{Uri.EscapeDataString(code)}");

    // ----- Collection Items -----

    public Task<ApiResult<List<CollectionItemDto>>> GetCollectionItemsAsync(string userName, string? search) =>
        SendAsync<List<CollectionItemDto>>(HttpMethod.Get,
            $"api/collectionitems?userName={Uri.EscapeDataString(userName)}&search={Uri.EscapeDataString(search ?? string.Empty)}");

    public Task<ApiResult<CollectionItemDto>> GetCollectionItemAsync(int id, string userName) =>
        SendAsync<CollectionItemDto>(HttpMethod.Get, $"api/collectionitems/{id}?userName={Uri.EscapeDataString(userName)}");

    public Task<ApiResult<CollectionItemDto>> CreateCollectionItemAsync(CollectionItemUpsertDto dto) =>
        SendAsync<CollectionItemUpsertDto, CollectionItemDto>(HttpMethod.Post, "api/collectionitems", dto);

    public Task<ApiResult<CollectionItemDto>> UpdateCollectionItemAsync(int id, CollectionItemUpsertDto dto) =>
        SendAsync<CollectionItemUpsertDto, CollectionItemDto>(HttpMethod.Put, $"api/collectionitems/{id}", dto);

    public Task<ApiResult<object?>> DeleteCollectionItemAsync(int id, string userName) =>
        SendAsync<object?>(HttpMethod.Delete, $"api/collectionitems/{id}?userName={Uri.EscapeDataString(userName)}");

    // ----- Analytics (Staff only) -----

    public Task<ApiResult<List<PopularItemDto>>> GetPopularItemsAsync(int top = 10) =>
        SendAsync<List<PopularItemDto>>(HttpMethod.Get, $"api/analytics/popular-items?top={top}");

    public Task<ApiResult<List<CategoryDemandDto>>> GetCategoryDemandAsync() =>
        SendAsync<List<CategoryDemandDto>>(HttpMethod.Get, "api/analytics/category-demand");

    public Task<ApiResult<List<UserActivityDto>>> GetUserActivityAsync(int top = 10) =>
        SendAsync<List<UserActivityDto>>(HttpMethod.Get, $"api/analytics/user-activity?top={top}");

    public Task<ApiResult<DashboardSummaryDto>> GetDashboardSummaryAsync() =>
        SendAsync<DashboardSummaryDto>(HttpMethod.Get, "api/analytics/summary");

    // ----- core plumbing -----

    private Task<ApiResult<TResponse>> SendAsync<TResponse>(HttpMethod method, string url) =>
        SendCoreAsync<object, TResponse>(method, url, null);

    private Task<ApiResult<TResponse>> SendAsync<TRequest, TResponse>(HttpMethod method, string url, TRequest body) =>
        SendCoreAsync<TRequest, TResponse>(method, url, body);

    private async Task<ApiResult<TResponse>> SendCoreAsync<TRequest, TResponse>(HttpMethod method, string url, TRequest? body)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, typeof(TRequest), options: JsonOptions);
        }

        using var response = await _http.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return ApiResult<TResponse>.Ok(default!, (int)response.StatusCode);
            }

            var data = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
            return ApiResult<TResponse>.Ok(data!, (int)response.StatusCode);
        }

        string? title = null;
        Dictionary<string, string[]>? errors = null;
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>(JsonOptions);
            title = problem?.Title;
            errors = problem?.Errors;
        }
        catch (JsonException)
        {
            // Response body wasn't ProblemDetails-shaped JSON; fall back to a generic message below.
        }

        return ApiResult<TResponse>.Fail((int)response.StatusCode, title ?? $"Request failed ({(int)response.StatusCode}).", errors);
    }

    private class ProblemDetailsResponse
    {
        public string? Title { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
