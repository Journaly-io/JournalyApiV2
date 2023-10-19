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

    public async Task CreateUser(string email, string password, string firstName, string lastName)
    {
        var request = new HttpRequestMessage(HttpMethod.Post,
            new Uri(_config.GetSection("IdentityStore").GetValue<string>("AdminEndpoint")));
        var body = new KeycloakCreateUserRequest
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Username = Guid.NewGuid().ToString(),
            Credentials = new KeycloakCredentials
            {
                Value = password
            }
        };
        request.Content = new StringContent(JsonSerializer.Serialize(body));
        await AuthenticatedRequest(request);
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
        }

        return response;
    }

    private async Task RefreshToken()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post,
            new Uri(new Uri(_config.GetSection("IdentityStore").GetValue<string>("Authority")),
                "protocol/openid-connect/auth")); // TODO: pull from .well_known
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("grant_type", "client_credentials"));
        collection.Add(new("client_id", _config.GetSection("IdentityStore").GetValue<string>("ClientId")));
        collection.Add(new("client_secret", _config.GetSection("IdentityStore").GetValue<string>("ClientSecret")));
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var payload = JsonSerializer.Deserialize<ClientCredentialsTokenResponseModel>(await response.Content.ReadAsStreamAsync());
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

    private class KeycloakCreateUserRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;
        
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [JsonPropertyName("credentials")]
        public KeycloakCredentials Credentials { get; set; }
    }

    private class KeycloakCredentials
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "password";
        
        [JsonPropertyName("value")]
        public string Value { get; set; }
        
        [JsonPropertyName("temporary")]
        public bool Temporary = false;
    }
}