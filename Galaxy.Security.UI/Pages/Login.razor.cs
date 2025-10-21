using Blazored.Toast.Services;
using Galaxy.Security.UI.Models;
using Galaxy.Security.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Galaxy.Security.UI.Pages
{
    public partial class Login
    {
        [Inject] HttpClient HttpClient { get; set; } = default!;
        [Inject] AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] IToastService Toast { get; set; } = default!;
        [Inject] NavigationManager Navegador { get; set; } = default!;
        public LoginRequest UserModel { get; set; } = new();

        public async Task OnLogin()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/Login")
                {
                    Content = JsonContent.Create(UserModel)
                };

                request.SetBrowserRequestOption("credentials", "include");

                var response = await HttpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var authService = (AuthenticationService)Auth;
                    authService.Authenticate();
                    Toast.ShowSuccess("Bienvenido al sistema de reclamos");
                    Navegador.NavigateTo("/");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Toast.ShowWarning("Usuario o contraseña incorrectos");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Toast.ShowError($"Hubo un error al iniciar sesión: {error}");
                }
            }
            catch (Exception ex)
            {
                Toast.ShowError($"Hubo un error al iniciar sesión: {ex.Message}");
            }
        }
    }
}
