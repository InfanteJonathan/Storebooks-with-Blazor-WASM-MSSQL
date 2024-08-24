using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using SoreBooksBlazorWASM.Service;
using StoreBooksBlazorWASM.Client;
using StoreBooksBlazorWASM.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddScoped<LibroService>();
builder.Services.AddScoped<VentasService>();
builder.Services.AddScoped<UsuarioServicio>();

builder.Services.AddFluentUIComponents();
builder.Services.AddDataGridEntityFrameworkAdapter();



builder.Services.AddScoped(http => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

await builder.Build().RunAsync();
