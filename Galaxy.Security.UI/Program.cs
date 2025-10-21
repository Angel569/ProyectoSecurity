using Blazored.Toast;
using Galaxy.Security.UI;
using Galaxy.Security.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var backendUrl = builder.Configuration.GetValue<string>("Services:UrlBackend");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(backendUrl!) });
builder.Services.AddBlazoredToast();

builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationService>();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
