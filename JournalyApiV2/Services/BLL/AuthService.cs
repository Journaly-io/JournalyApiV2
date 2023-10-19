using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JournalyApiV2.Services.BLL;

public class AuthService
{
    private static string token;
    private static DateTime tokenExpiration;

    private readonly IConfiguration _config;

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    private async Task<HttpResponseMessage?> AuthenticatedRequest(HttpRequestMessage request)
    {
        if (tokenExpiration >= DateTime.Now) await RefreshToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var client = new HttpClient();
        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Refresh and try again
            await RefreshToken();
            response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        return response;
    }
    
    private async Task RefreshToken()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(_config.GetSection("IdentityStore").GetValue<string>("AdminEndpoint")), "users"));
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("grant_type", "client_credentials"));
        collection.Add(new("client_id", _config.GetSection("IdentityStore").GetValue<string>("ClientId")));
        collection.Add(new("client_secret", _config.GetSection("IdentityStore").GetValue<string>("ClientSecret")));
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var payload = await JsonSerializer.DeserializeAsync<ClientCredentialsTokenResponseModel>(await response.Content.ReadAsStreamAsync());
        token = payload.AccessToken;
        var expiration = DateTime.Now;
        expiration.AddSeconds(payload.ExpiresIn);
        tokenExpiration = expiration;
    }

    private class ClientCredentialsTokenResponseModel
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        
        [JsonPropertyName("not-before-policy")]
        public int NotBeforePolicy { get; set; }

        [JsonPropertyName("scope")] 
        public string Scope { get; set; }
    }
}