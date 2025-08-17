using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; 
using webapi.Repositories;
using webapi.Services;
using webapi.Data;
using webapi.Middleware;
using webapi.Extensions; 
using FluentValidation;
using webapi.Validators;
using webapi.Authorization;
using webapi.Models; 
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IVisitTypeRepository, VisitTypeRepository>();
builder.Services.AddScoped<IPatientVisitRepository, PatientVisitRepository>();
builder.Services.AddScoped<IFeeRateRepository, FeeRateRepository>();
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddScoped<IForgotPasswordRepository, ForgotPasswordRepository>();

builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddPolicies();

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

try
{
    Log.Information("Starting web application");

    app.UseExceptionHandling();
    app.ConfigureRequestLogging();
    app.UseRequestLogging();

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

    using (var scope = app.Services.CreateScope())
    {
        var dbFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        try
        {
            using var connection = dbFactory.CreateConnection();
            connection.Open();
            Log.Information("Database connection successful");
            Console.WriteLine("✅ Database connection successful!");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Database connection failed");
            Console.WriteLine("❌ Database connection failed: " + ex.Message);
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}