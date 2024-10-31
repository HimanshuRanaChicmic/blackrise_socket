using BlackRise.Identity.Persistence;
using BlackRise.Identity.Application;
using BlackRise.Identity.Extensions;
using BlackRise.Identity.Middleware;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;
using BlackRise.Identity.Application.DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using BlackRise.Identity.HttpClient;

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

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.UseCustomExceptionHandler();

app.MapControllers();

app.MigrateDatabase();

app.Run();
