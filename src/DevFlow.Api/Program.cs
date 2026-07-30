using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using DevFlow.Application;
using DevFlow.Infrastructure.Data;
using DevFlow.Application.Interfaces.Usuarios;
using DevFlow.Infrastructure.Repositories.Usuarios;
using DevFlow.Application.Interfaces;
using DevFlow.Infrastructure.Repositories.Projetos;
using DevFlow.Infrastructure.Repositories.Tarefas;

var builder = WebApplication.CreateBuilder(args);

// Hospedagens como o Render informam a porta via variável de ambiente PORT.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApplication();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProjetosRepository, ProjetoRepository>();
builder.Services.AddScoped<ITarefasRepository, TarefaRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtKey = builder.Configuration["Jwt:Key"]!;
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevFlowCors", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:4200",       // Angular em dev
                  "https://leosimioni21.github.io" // front publicado no GitHub Pages
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

// Sem isso, uma exceção não tratada gera um 500 sem cabeçalho de CORS,
// e o navegador reporta "bloqueado por CORS" escondendo o erro real.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        app.Logger.LogError(feature?.Error, "Erro não tratado em {Path}", context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { erro = "Ocorreu um erro inesperado no servidor." });
    });
});

app.UseCors("DevFlowCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
