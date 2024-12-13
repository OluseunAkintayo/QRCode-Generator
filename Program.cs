using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QRC.Services;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

string[] allowedLocations = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>()!;

builder.Services.AddCors(options => {
  options.AddPolicy("AllowedOrigins", policy => {
    policy.WithOrigins(allowedLocations).AllowAnyHeader().AllowAnyMethod();
  });
});

// Add services to the container.

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DbService>(options => {
  options.UseMySql(
    builder.Configuration.GetConnectionString("Sql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Sql"))
  );
});

builder.Services.AddSwaggerGen(options => {
  options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
    In  = ParameterLocation.Header,
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey
  });
  options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(auth => {
  auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
  options.TokenValidationParameters = new TokenValidationParameters {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
  };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<DbService>();
builder.Services.AddTransient<QrService>();
builder.Services.AddTransient<AuthService>();

var app = builder.Build();

app.UseCors("AllowedOrigins");

app.UseStaticFiles();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
// }


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

