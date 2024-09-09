using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QRC.Services;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

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
  options.UseSqlite(builder.Configuration.GetConnectionString("Nexus"));
});

builder.Services.AddSwaggerGen(options => {
  options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
    In  = ParameterLocation.Header,
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Description = "Bearer [token]"
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

string[] allowedLocations = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>()!;
builder.Services.AddCors(options => {
  options.AddPolicy("AllowLocalOrigin", policy => {
    policy.WithOrigins(allowedLocations).AllowAnyHeader().AllowAnyMethod();
  });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<DbService>();
builder.Services.AddTransient<QrService>();
builder.Services.AddTransient<AuthService>();

var app = builder.Build();

// using (var scope  = app.Services.CreateScope()) {
//   var dbContext = scope.ServiceProvider.GetRequiredService<DbService>();
//   dbContext.Database.Migrate();
// }

app.UseStaticFiles();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
// }

app.UseCors("AllowLocalOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

