using BlackRise.Identity.Application;
using BlackRise.Identity.Application.DataTransferObject;
using BlackRise.Identity.Extensions;
using BlackRise.Identity.HttpClient;
using BlackRise.Identity.Middleware;
using BlackRise.Identity.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using DotNetEnv;
Env.Load();

var builder = WebApplication.CreateBuilder(args);

var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "";
var dbUser = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "";
var dbPass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";


string defaultConnStr = $"Host={dbHost};Database={dbName};Username={dbUser};Password={dbPass}";

// Log all DB env variables for testing
Console.WriteLine($"DefaultConnectionString: {defaultConnStr}");

builder.Configuration["ConnectionStrings:DefaultConnectionString"] = defaultConnStr;

builder.Configuration["EmailSettings:Password"] = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
builder.Configuration["EmailSettings:Host"] = Environment.GetEnvironmentVariable("EMAIL_HOST");
builder.Configuration["EmailSettings:Port"] = Environment.GetEnvironmentVariable("EMAIL_PORT");

builder.Configuration["LinkedInSetting:LinkedInClientId"] = Environment.GetEnvironmentVariable("LINKEDIN_CLIENT_ID");
builder.Configuration["LinkedInSetting:LinkedInClientSecret"] = Environment.GetEnvironmentVariable("LINKEDIN_CLIENT_SECRET");
builder.Configuration["LinkedInSetting:LinkedInRedirectUri"] = Environment.GetEnvironmentVariable("LINKEDIN_REDIRECT_URI");

builder.Configuration["JwtSettings:Key"] = Environment.GetEnvironmentVariable("JWT_KEY");
builder.Configuration["JwtSettings:LinkedInClientId"] = Environment.GetEnvironmentVariable("JWT_LINKEDIN_CLIENT_ID");
builder.Configuration["JwtSettings:LinkedInClientSecret"] = Environment.GetEnvironmentVariable("JWT_LINKEDIN_CLIENT_SECRET");

// Set ClientUrlSettings from environment variables
builder.Configuration["ClientUrlSettings:EmailConfirmation"] = Environment.GetEnvironmentVariable("CLIENT_URL_EMAIL_CONFIRMATION");
builder.Configuration["ClientUrlSettings:ResetPassword"] = Environment.GetEnvironmentVariable("CLIENT_URL_RESET_PASSWORD");
builder.Configuration["ClientUrlSettings:Login"] = Environment.GetEnvironmentVariable("CLIENT_URL_LOGIN");
builder.Configuration["ClientUrlSettings:LoginRedirect"] = Environment.GetEnvironmentVariable("CLIENT_URL_LOGIN_REDIRECT");
builder.Configuration["ClientUrlSettings:SenderUrl"] = Environment.GetEnvironmentVariable("CLIENT_URL_SENDER_URL");
builder.Configuration["ClientUrlSettings:ProfileUrl"] = Environment.GetEnvironmentVariable("CLIENT_URL_PROFILE_URL");

// Log all new env variables for testing
Console.WriteLine($"ClientUrlSettings:EmailConfirmation: {builder.Configuration["ClientUrlSettings:EmailConfirmation"]}");
Console.WriteLine($"ClientUrlSettings:ResetPassword: {builder.Configuration["ClientUrlSettings:ResetPassword"]}");
Console.WriteLine($"ClientUrlSettings:Login: {builder.Configuration["ClientUrlSettings:Login"]}");
Console.WriteLine($"ClientUrlSettings:LoginRedirect: {builder.Configuration["ClientUrlSettings:LoginRedirect"]}");
Console.WriteLine($"ClientUrlSettings:SenderUrl: {builder.Configuration["ClientUrlSettings:SenderUrl"]}");
Console.WriteLine($"ClientUrlSettings:ProfileUrl: {builder.Configuration["ClientUrlSettings:ProfileUrl"]}");
Console.WriteLine($"ClientUrlSettings:EmailConfirmationTokenExpire: {builder.Configuration["ClientUrlSettings:EmailConfirmationTokenExpire"]}");
Console.WriteLine($"ClientUrlSettings:ResetPasswordTokenExpire: {builder.Configuration["ClientUrlSettings:ResetPasswordTokenExpire"]}");
Console.WriteLine($"LinkedInSetting:LinkedInRedirectUri: {builder.Configuration["LinkedInSetting:LinkedInRedirectUri"]}");

builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            IEnumerable<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError> errors = context.ModelState.Values
            .SelectMany(v => v.Errors);
            List<string> errorMessages = errors.Select(x => x.ErrorMessage).ToList();
            return new BadRequestObjectResult(new ErrorDto(errorMessages));
        };
    });
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddTransient<IValidatorInterceptor, UseCustomErrorModelInterceptor>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClientServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddIdentityServices(builder);
builder.Services.AddHealthChecks();


var result = builder.Configuration.GetSection("CorsUrls").Value;
var urls = (result != null && result.Split(',').Any()) ? result.Split(',') : Array.Empty<string>();

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

//builder.Services.AddCors(options =>
//{
//    // allowed all for testing purposes
//    options.AddPolicy("AllowAll",
//                        policy =>
//                        {
//                            //policy.WithOrigins(urls)
//                            policy.AllowAnyOrigin()
//                                  .AllowAnyHeader()
//                                  .AllowAnyMethod();
//                                  //.AllowCredentials();
//                        });
//});

var app = builder.Build();

app.MapHealthChecks("/security-service/health");

app.UseSwagger(c =>
{
    c.RouteTemplate = "security-service/swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/security-service/swagger/v1/swagger.json", "BlackRise Security Service");
    c.RoutePrefix = "security-service/swagger";
});

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();
//// allowed all for testing purposes
//app.UseCors("AllowAll");
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers["Origin"];
    if (!string.IsNullOrEmpty(origin))
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS, PATCH");
    }

    // Handle preflight request
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 204;
        return;
    }

    await next();
});
app.UseAuthentication();
app.UseAuthorization();

app.UseCustomExceptionHandler();

app.MapControllers();

app.MigrateDatabase();

app.Run();
