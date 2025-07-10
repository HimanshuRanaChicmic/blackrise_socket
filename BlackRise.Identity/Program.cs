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
Console.WriteLine($"DB_HOST: {dbHost}");
Console.WriteLine($"DB_NAME: {dbName}");
Console.WriteLine($"DB_USERNAME: {dbUser}");
Console.WriteLine($"DB_PASSWORD: {dbPass}");

// Log all Email env variables for testing
Console.WriteLine($"EMAIL_PASSWORD: {Environment.GetEnvironmentVariable("EMAIL_PASSWORD")}");
Console.WriteLine($"EMAIL_HOST: {Environment.GetEnvironmentVariable("EMAIL_HOST")}");
Console.WriteLine($"EMAIL_PORT: {Environment.GetEnvironmentVariable("EMAIL_PORT")}");

// Log all LinkedIn env variables for testing
Console.WriteLine($"LINKEDIN_CLIENT_ID: {Environment.GetEnvironmentVariable("LINKEDIN_CLIENT_ID")}");
Console.WriteLine($"LINKEDIN_CLIENT_SECRET: {Environment.GetEnvironmentVariable("LINKEDIN_CLIENT_SECRET")}");

// Log all JWT env variables for testing
Console.WriteLine($"JWT_KEY: {Environment.GetEnvironmentVariable("JWT_KEY")}");
Console.WriteLine($"JWT_LINKEDIN_CLIENT_ID: {Environment.GetEnvironmentVariable("JWT_LINKEDIN_CLIENT_ID")}");

Console.WriteLine($"JWT_LINKEDIN_CLIENT_SECRET: {Environment.GetEnvironmentVariable("JWT_LINKEDIN_CLIENT_SECRET")}");


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
