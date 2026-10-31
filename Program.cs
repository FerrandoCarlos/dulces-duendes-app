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
using DulcesDuendesApp.Common.Auth;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
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
builder.Services.AddScoped<IDetalleTicketRepository, DetalleTicketRepository>();
builder.Services.AddScoped<IDetalleTicketService, DetalleTicketService>();
builder.Services.AddScoped<IImagenProductoRepository, ImagenProductoRepository>();
builder.Services.AddScoped<IImagenProductoService, ImagenProductoService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEstadisticasRepository, EstadisticasRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Auth Jwt
var jwt = builder.Configuration.GetSection(JwtOptions.Seccion).Get<JwtOptions>() ?? new JwtOptions();
if (jwt.Clave.Length < 32) throw new InvalidOperationException("Falta Jwt:Clave o es demasiado corta (mínimo 32 caracteres). Definila en appsettings.Development.json o con la variable de entorno Jwt__Clave.");
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Seccion));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Events.OnRedirectToLogin = AuthEventos.AlRedirigirALogin;
    options.Events.OnRedirectToAccessDenied = AuthEventos.AlRedirigirAAccesoDenegado;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Emisor,
        ValidateAudience = true,
        ValidAudience = jwt.Audiencia,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
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
            new Rol { Nombre = NombresRol.Administrador },
            new Rol { Nombre = NombresRol.Empleado }
        );
        await context.SaveChangesAsync();
    }

    if (await usuarioService.ObtenerCantidad() == 0)
    {
        var rolAdmin = context.Roles.First(r => r.Nombre == NombresRol.Administrador);

        var admin = new Usuario
        {
            Email = "admin@dulcesduendes.com",
            Nombre = "Admin",
            Apellido = "Sistema",
            RolId = rolAdmin.Id
        };
        await usuarioService.Alta(admin, "Admin123!");
    }
    if (!await context.Usuarios.AnyAsync(u => u.Email == "empleado@dulcesduendes.com"))
    {
        var rolEmpleado = context.Roles.First(r => r.Nombre == NombresRol.Empleado);

        await usuarioService.Alta(new Usuario
        {
            Email = "empleado@dulcesduendes.com",
            Nombre = "Empleado",
            Apellido = "Demo",
            RolId = rolEmpleado.Id
        }, "Empleado123!");
    }
}
