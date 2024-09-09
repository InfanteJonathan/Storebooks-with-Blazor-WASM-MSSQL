using Blazorise;
using Blazorise.FluentUI2;
using Blazorise.Icons.FluentUI;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StoreBooksBlazorWASM.Client;
using StoreBooksBlazorWASM.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddScoped<LibroService>();
builder.Services.AddScoped<VentasService>();
builder.Services.AddScoped<UsuarioServicio>();

builder.Services
    .AddBlazorise()
    .AddFluentUI2Providers()
    .AddFluentUIIcons();


builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("AdministradorPolicy", policy =>
        policy.RequireRole("Administrador"));
    options.AddPolicy("EmpleadoPolicy", policy =>
        policy.RequireRole("Empleado"));
    options.AddPolicy("UsuarioPolicy", policy =>
        policy.RequireRole("Usuario"));
});




builder.Services.AddScoped(http => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

await builder.Build().RunAsync();


