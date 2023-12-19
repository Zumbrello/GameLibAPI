using System.Text;
using GameLib;
using GameLib.Context;
using GameLib.Properties;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<GtcymkznContext>();

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => 
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        
        Version = "v1",
        Title = "GameLib",
        Description = "ASP.NET Core 7.0 Web API"

        
    });
    swagger.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme
    {
        In=ParameterLocation.Header,
        Description = "Please insert token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

string conectString = "Host=snuffleupagus.db.elephantsql.com;Database=gtcymkzn;Username=gtcymkzn;password=1FgXRFfmDumZegpQ-cnmPs-VpcMfRm1L";
builder.Services.AddDbContext<GtcymkznContext>(x => x.UseNpgsql(conectString));
// Регистрация политики авторизации
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserIdPolicy", policy =>
    {
        policy.Requirements.Add(new UserIdRequirement(10)); // Здесь 10 - требуемое значение userId
    });
});

// Регистрация обработчика требования
builder.Services.AddSingleton<IAuthorizationHandler, UserIdRequirementHandler>();

// Add JWT Authentication Middleware - This code will intercept HTTP request and validate the JWT.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    opt => {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }
);
var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors(policy =>
    policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
);
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
    {
        await context.Response.WriteAsync("Token Validation Has Failed. Request Access Denied");
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();