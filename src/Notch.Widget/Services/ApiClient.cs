using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Notch.Shared.Dto;

namespace Notch.Widget.Services;

public static class ApiClient
{
    private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5105") };

    public static async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> buildRequest)
    {
        var tokens = TokenStorage.Load() ?? throw new InvalidOperationException("Not logged in");
        var response = await SendWithTokenAsync(buildRequest, tokens.AccessToken);
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
       return await SendWithTokenAsync(buildRequest, refreshedTokens.AccessToken);
    }

    private static Task<HttpResponseMessage> SendWithTokenAsync(Func<HttpRequestMessage> buildRequest,
        string accessToken)
    {
        var request = buildRequest();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return _httpClient.SendAsync(request);
    }

    public static async Task<HttpResponseMessage> GetAsync(string url)
    {
        return await SendAsync(()=>  new HttpRequestMessage(HttpMethod.Get, url));
    }
    
    public static async Task<HttpResponseMessage> PostAsync(string url)
    {
        return await SendAsync(()=>  new HttpRequestMessage(HttpMethod.Post, url));
    }
    
    public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T body)
    {
        return await SendAsync(()=>  new HttpRequestMessage(HttpMethod.Post, url){Content = JsonContent.Create(body)});
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