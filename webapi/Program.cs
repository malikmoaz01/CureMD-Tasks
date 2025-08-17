// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.OpenApi.Models;
// using webapi.Repositories;
// using webapi.Services;
// using webapi.Data;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using System.Data;
// using Microsoft.Data.SqlClient;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

// builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
// builder.Services.AddScoped<IPatientRepository, PatientRepository>();
// builder.Services.AddScoped<IVisitTypeRepository, VisitTypeRepository>();
// builder.Services.AddScoped<IPatientVisitRepository, PatientVisitRepository>();
// builder.Services.AddScoped<IFeeRateRepository, FeeRateRepository>();
// builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
 

// builder.Services.AddScoped<IForgotPasswordRepository, ForgotPasswordRepository>();
// builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();

// builder.Services.AddScoped<IAuthService, AuthService>(); 

// var jwtSettings = builder.Configuration.GetSection("JwtSettings");
// var secretKey = jwtSettings["SecretKey"];

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = jwtSettings["Issuer"],
//             ValidAudience = jwtSettings["Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
//         };
//     });

// builder.Services.AddAuthorization();

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll",
//         policyBuilder =>
//         {
//             policyBuilder.AllowAnyOrigin()
//                          .AllowAnyMethod()
//                          .AllowAnyHeader();
//         });
// });

// var app = builder.Build();

// if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseCors("AllowAll");
// app.UseAuthentication();
// app.UseAuthorization();

// app.MapControllers();

// // --- DB connection test ---
// using (var scope = app.Services.CreateScope())
// {
//     var dbFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
//     try
//     {
//         using var connection = dbFactory.CreateConnection();
//         connection.Open();
//         Console.WriteLine("✅ Database connection successful!");
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine("❌ Database connection failed: " + ex.Message);
//     }
// }

// app.Run();


using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using webapi.Repositories;
using webapi.Services;
using webapi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using FluentValidation;
using webapi.Validators;
using webapi.Authorization;
using webapi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

// --- Repositories ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IVisitTypeRepository, VisitTypeRepository>();
builder.Services.AddScoped<IPatientVisitRepository, PatientVisitRepository>();
builder.Services.AddScoped<IFeeRateRepository, FeeRateRepository>();
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddScoped<IForgotPasswordRepository, ForgotPasswordRepository>();

// --- Services ---
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// --- FluentValidation Validators ---
builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
builder.Services.AddScoped<IValidator<ForgotPasswordRequestDto>, ForgotPasswordRequestDtoValidator>();
builder.Services.AddScoped<IValidator<VerifyOtpDto>, VerifyOtpDtoValidator>();
builder.Services.AddScoped<IValidator<ResetPasswordDto>, ResetPasswordDtoValidator>();
builder.Services.AddScoped<IValidator<CreateDoctorDto>, CreateDoctorDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateDoctorDto>, UpdateDoctorDtoValidator>();
builder.Services.AddScoped<IValidator<CreatePatientDto>, CreatePatientDtoValidator>();
builder.Services.AddScoped<IValidator<UpdatePatientDto>, UpdatePatientDtoValidator>();
builder.Services.AddScoped<IValidator<CreatePatientVisitDto>, CreatePatientVisitDtoValidator>();
builder.Services.AddScoped<IValidator<UpdatePatientVisitDto>, UpdatePatientVisitDtoValidator>();
builder.Services.AddScoped<IValidator<CreateVisitTypeDto>, CreateVisitTypeDtoValidator>();
builder.Services.AddScoped<IValidator<VisitType>, VisitTypeValidator>();
builder.Services.AddScoped<IValidator<FeeRate>, FeeRateValidator>();
builder.Services.AddScoped<IValidator<ActivityLog>, ActivityLogValidator>();

// --- JWT Authentication ---
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// --- Authorization Policies ---
builder.Services.AddPolicies();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- DB connection test ---
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
    try
    {
        using var connection = dbFactory.CreateConnection();
        connection.Open();
        Console.WriteLine("✅ Database connection successful!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Database connection failed: " + ex.Message);
    }
}

app.Run();
