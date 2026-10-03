using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Notch.Shared.Dto;

namespace Notch.Widget.Services;

public static class ApiClient
{
    private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5105") };

    public static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        var tokens = TokenStorage.Load();
        if (tokens is null)
        {
            throw new InvalidOperationException("Not logged in");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        var response = await _httpClient.SendAsync(request);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        await RefreshToken();
        var refreshedTokens = TokenStorage.Load();
        if (refreshedTokens is null)
        {
            return response;
        }
        var retryRequest = new HttpRequestMessage(request.Method, request.RequestUri);
        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshedTokens.AccessToken);
        return await _httpClient.SendAsync(retryRequest);
    }

    private static async Task RefreshToken()
    {
        var tokens = TokenStorage.Load();
        if (tokens is null) return;
        var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", new RefreshRequest(tokens.RefreshToken));
        if (response.IsSuccessStatusCode)
        {
            var newTokens  = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (newTokens is not null)
            {
                TokenStorage.Save(newTokens);
            }
        }
    }
}