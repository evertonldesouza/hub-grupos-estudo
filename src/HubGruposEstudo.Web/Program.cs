using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using HubGruposEstudo.Infrastructure.Data;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Interfaces;
using HubGruposEstudo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:SecretKey"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:SecretKey deve ser configurada e possuir pelo menos 32 caracteres. " +
        "Use a variável de ambiente Jwt__SecretKey em ambientes não locais.");
}

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hub de Grupos de Estudo",
        Version = "v1",
        Description = "API para criação de trilhas, progresso de estudos e trocas de conhecimento."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe apenas o token JWT. Exemplo: Bearer {token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection não está configurada");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IHabilidadeRepository, HabilidadeRepository>();
builder.Services.AddScoped<ITrilhaRepository, TrilhaRepository>();
builder.Services.AddScoped<IInscricaoRepository, InscricaoRepository>();
builder.Services.AddScoped<ITrocaRepository, TrocaRepository>();
builder.Services.AddScoped<IConquistaRepository, ConquistaRepository>();
builder.Services.AddScoped<IUsuarioHabilidadeRepository, UsuarioHabilidadeRepository>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<HabilidadeService>();
builder.Services.AddScoped<TrilhaService>();
builder.Services.AddScoped<InscricaoService>();
builder.Services.AddScoped<TrocaService>();
builder.Services.AddScoped<ConquistaService>();

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
if (string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
    throw new InvalidOperationException("Jwt:Issuer e Jwt:Audience devem ser configurados");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
        limiterOptions.AutoReplenishment = true;
    });
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
    await DbInitializer.InitializeAsync(app.Services, app.Configuration, app.Logger);

var useHttpsRedirection = false;
if (bool.TryParse(app.Configuration["UseHttpsRedirection"], out var configuredHttps))
    useHttpsRedirection = configuredHttps;

if (useHttpsRedirection)
    app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", async (AppDbContext db) =>
{
    try
    {
        var conectado = await db.Database.CanConnectAsync();
        return conectado
            ? (IResult)Results.Ok(new { status = "ok" })
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
    catch
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
}).AllowAnonymous();

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }
