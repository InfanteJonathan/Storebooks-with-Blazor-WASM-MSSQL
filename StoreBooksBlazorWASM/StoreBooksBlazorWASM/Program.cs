using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreBooksBlazorWASM.Components;
using StoreBooksBlazorWASM.Components.Account;
using StoreBooksBlazorWASM.Data;
using StoreBooksBlazorWASM.Service;
using StoreBooksBlazorWASM.Client.Services;
using SoreBooksBlazorWASM.Service;
using Blazorise;
using Blazorise.FluentUI2;
using Blazorise.Icons.FluentUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexionSQL")));



builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();


builder.Services.AddScoped<LibroManagerV1>();
builder.Services.AddScoped<LibroService>();
builder.Services.AddScoped<VentasManagerV1>();
builder.Services.AddScoped<VentasService>();
builder.Services.AddScoped<UsuarioServicio>();
builder.Services.AddScoped<UsuarioManagerV1>();

builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();





builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();



builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UsuarioPolicy", policy =>
        policy.RequireRole("Usuario"))
    .AddPolicy("AdministradorPolicy", policy =>
        policy.RequireRole("Administrador"))
    .AddPolicy("EmpleadoPolicy", policy =>
        policy.RequireRole("Empleado"))
    .AddPolicy("AdminEmpleado", policy =>
        policy.RequireRole("Administrador", "Empleado"));


builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services
    .AddBlazorise()
    .AddFluentUI2Providers()
    .AddFluentUIIcons();





builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


builder.Services.AddScoped(http => new HttpClient
{
    BaseAddress = new Uri("https://localhost:44348")
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(StoreBooksBlazorWASM.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
