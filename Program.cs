using Microsoft.EntityFrameworkCore;
using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Repositories.Implementations;
using DulcesDuendesApp.Services.Interfaces;
using DulcesDuendesApp.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using DulcesDuendesApp.Common.Configuration;
using DulcesDuendesApp.Common.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IPrecioCalculador, PrecioCalculador>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<AppExceptionFilter>();
builder.Services.AddScoped<ITipoProductoRepository, TipoProductoRepository>();
builder.Services.AddScoped<ITipoProductoService, TipoProductoService>();
builder.Services.AddScoped<IAlmacenArchivos, AlmacenArchivos>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {

        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = AuthEventos.AlRedirigirALogin;
        options.Events.OnRedirectToAccessDenied = AuthEventos.AlRedirigirAAccesoDenegado;
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.Configure<NegocioOptions>(
    builder.Configuration.GetSection(NegocioOptions.Seccion));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var usuarioService = scope.ServiceProvider.GetRequiredService<IUsuarioService>();
    //await context.Database.MigrateAsync();
    SeedAsync(context, usuarioService).GetAwaiter().GetResult();
}

app.Run();

static async Task SeedAsync(ApplicationDbContext context, IUsuarioService usuarioService)
{
    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Rol { Nombre = "Administrador" },
            new Rol { Nombre = "Empleado" }
        );
        await context.SaveChangesAsync();
    }

    if (await usuarioService.ObtenerCantidad() == 0)
    {
        var rolAdmin = context.Roles.First(r => r.Nombre == "Administrador");

        var admin = new Usuario
        {
            Email = "admin@dulcesduendes.com",
            Nombre = "Admin",
            Apellido = "Sistema",
            RolId = rolAdmin.Id
        };
        await usuarioService.Alta(admin, "Admin123!");
    }
}
