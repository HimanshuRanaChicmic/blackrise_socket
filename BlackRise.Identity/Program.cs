using BlackRise.Identity.Application;
using BlackRise.Identity.Application.DataTransferObject;
using BlackRise.Identity.Extensions;
using BlackRise.Identity.HttpClient;
using BlackRise.Identity.Middleware;
using BlackRise.Identity.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
                        policy =>
                        {
                            policy.WithOrigins(urls)
                                  .AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowCredentials();
                        });
});

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
app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.UseCustomExceptionHandler();

app.MapControllers();

app.MigrateDatabase();

app.Run();
