using Galaxy.Security.UI.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Galaxy.Security.UI.Services
{
    public class AuthenticationService : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ClaimsPrincipal _claimsPrincipal = new(new ClaimsIdentity());
        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public void Authenticate()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        public async Task Logout()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/Logout");
            request.SetBrowserRequestOption("credentials", "include"); 

            var response = await _httpClient.SendAsync(request);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/api/Auth/Me");
                request.SetBrowserRequestOption("credentials", "include"); //COOKIES

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return new AuthenticationState(_claimsPrincipal);
                }
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<ClaimDto>>>(options);
                if(result!.IsSuccess == false || result!.Result == null)
                {
                    return new AuthenticationState(_claimsPrincipal);
                }
                var claims = result.Result.Select(c => new Claim(c.Type, c.Value)).ToList();
                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                return new AuthenticationState(principal);
            }
            catch (Exception)
            {
                return new AuthenticationState(_claimsPrincipal);
            }
        }
    }
}
