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

app.UseHttpsRedirection();

// Para que funcione la authenticacion con JWT
app.UseAuthentication();

app.Run();

