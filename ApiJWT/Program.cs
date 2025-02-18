using System.Text;
using ApiJWT.Custom;
using ApiJWT.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
/* builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); */


builder.Services.AddDbContext<ProductosJwtContext>(opt => {
    string conecString = builder.Configuration.GetConnectionString("SqlServerCon")!;
    opt.UseSqlServer(conecString); 
});

// Agregando como dependencias las funciones de la clase utilidades
builder.Services.AddSingleton<Utilities>();

// Agregando como dependencias el jwt y sus configuraciones
builder.Services.AddAuthentication(conf => {
    conf.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    conf.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config => {
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["JWT:key"]!))
    };
});

builder.Services.AddCors(opt => {
    opt.AddPolicy("GeneralPolicy", app => {
        app
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    /* app.UseSwagger(); */
    /* app.UseSwaggerUI(); */
    app.MapOpenApi();
    app.UseSwaggerUi(opt => {
        opt.Path = "/openapi";
        opt.DocumentPath = "/openapi/v1.json";
    });
}

// Habilitar los Cors Configurados
app.UseCors("GeneralPolicy");

app.UseHttpsRedirection();

app.MapControllers();

// Para que funcione la authenticacion con JWT
// Sin app.UseAuthentication();, el usuario nunca se autenticaría y el [Authorize] no funcionaría.
// verifica si la solicitud HTTP entrante incluye credenciales de autenticación válidas (como un token JWT, una cookie de autenticación, etc.).
app.UseAuthentication();

// Sin app.UseAuthorization();, aunque el usuario esté autenticado, no se aplicaría ninguna restricción de acceso.
// verifica si el usuario autenticado tiene los permisos necesarios para acceder a un recurso
app.UseAuthorization();


app.Run();

