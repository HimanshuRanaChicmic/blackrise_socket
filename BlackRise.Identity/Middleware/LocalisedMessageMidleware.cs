using BlackRise.Identity.Application.DataTransferObject;
using BlackRise.Identity.Application.Exceptions;
using BlackRise.Identity.Application.Resources;
using BlackRise.Identity.Application.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Globalization;
using Microsoft.Extensions.Logging;
using BlackRise.Identity.Application.Utils;
using BlackRise.Identity.Application.Helpers;

namespace BlackRise.Identity.Middleware;

public class LocalizedMessageMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LocalizedMessageMiddleware> _logger;
    public LocalizedMessageMiddleware(RequestDelegate next, ILogger<LocalizedMessageMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        try
        {
            await _next(context);

            memStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memStream).ReadToEndAsync();
            memStream.Seek(0, SeekOrigin.Begin);

            string modifiedBody = responseBody;
            if (!string.IsNullOrWhiteSpace(responseBody) && context.Response.ContentType != null && context.Response.ContentType.Contains("application/json"))
            {
                try
                {
                    var jToken = JToken.Parse(responseBody);
                    bool changed = false;

                    // If message is empty and result is a string, localize result and set as message
                    if ((jToken["message"] == null || string.IsNullOrWhiteSpace(jToken["message"].ToString())) &&
                        jToken["result"] != null && jToken["result"].Type == JTokenType.String)
                    {
                        var originalMsg = jToken["result"].ToString();
                        var localizedMsg = LocalizationHelper.GetLocalizedMessageFromConstantValue(originalMsg);
                        _logger.LogInformation($"[Localization] (result->message) Original='{originalMsg}', Localized='{localizedMsg}'");
                        jToken["message"] = localizedMsg;

                        // Set result to English (default) value
                        var key = LocalizationHelper.GetConstantKey(originalMsg);
                        var englishMsg = LocalizationHelper.GetLocalizedMessageFromKey(key, new System.Globalization.CultureInfo("en"));
                        jToken["result"] = englishMsg;
                        changed = true;
                    }

                    // Ensure 'result' is always in English if it matches the OTP message pattern
                    if (jToken["result"] != null && jToken["result"].Type == JTokenType.String)
                    {
                        var resultValue = jToken["result"].ToString();
                        var otpPrefixLocalized = LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OtpSentSuccessfully);
                        var otpPrefixEnglish = LocalizationHelper.GetLocalizedMessageFromKey(LocalizationHelper.GetConstantKey(Constants.OtpSentSuccessfully), new System.Globalization.CultureInfo("en"));
                        if (resultValue.StartsWith(otpPrefixLocalized))
                        {
                            // Extract the code part (e.g., ": 663027")
                            var codePart = resultValue.Substring(otpPrefixLocalized.Length);
                            jToken["result"] = otpPrefixEnglish + codePart;
                            changed = true;
                        }
                    }

                    void LocalizeFields(JToken token)
                    {
                        if (token.Type == JTokenType.Object)
                        {
                            var obj = (JObject)token;
                            foreach (var property in obj.Properties())
                            {
                                if (property.Name.Equals("message", StringComparison.OrdinalIgnoreCase) && property.Value.Type == JTokenType.String)
                                {
                                    var originalMsg = property.Value.ToString();
                                    var localizedMsg = LocalizationHelper.GetLocalizedMessageFromConstantValue(originalMsg);
                                    _logger.LogInformation($"[Localization] message: Original='{originalMsg}', Localized='{localizedMsg}'");
                                    if (localizedMsg != originalMsg)
                                    {
                                        property.Value = localizedMsg;
                                        changed = true;
                                    }
                                }
                                else if (property.Name.Equals("error", StringComparison.OrdinalIgnoreCase) && property.Value.Type == JTokenType.String)
                                {
                                    var originalMsg = property.Value.ToString();
                                    var localizedMsg = LocalizationHelper.GetLocalizedMessageFromConstantValue(originalMsg);
                                    _logger.LogInformation($"[Localization] error: Original='{originalMsg}', Localized='{localizedMsg}'");
                                    if (localizedMsg != originalMsg)
                                    {
                                        property.Value = localizedMsg;
                                        changed = true;
                                    }
                                }
                                else if (property.Name.Equals("validationErrors", StringComparison.OrdinalIgnoreCase) && property.Value.Type == JTokenType.Array)
                                {
                                    var arr = (JArray)property.Value;
                                    for (int i = 0; i < arr.Count; i++)
                                    {
                                        var originalMsg = arr[i]?.ToString();
                                        if (!string.IsNullOrEmpty(originalMsg))
                                        {
                                            var localizedMsg = LocalizationHelper.GetLocalizedMessageFromConstantValue(originalMsg);
                                            _logger.LogInformation($"[Localization] validationError[{i}]: Original='{originalMsg}', Localized='{localizedMsg}'");
                                            if (localizedMsg != originalMsg)
                                            {
                                                arr[i] = localizedMsg;
                                                changed = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Recursively localize nested objects/arrays
                                    LocalizeFields(property.Value);
                                }
                            }
                        }
                        else if (token.Type == JTokenType.Array)
                        {
                            foreach (var item in (JArray)token)
                            {
                                LocalizeFields(item);
                            }
                        }
                    }

                    LocalizeFields(jToken);

                    if (changed)
                    {
                        modifiedBody = jToken.ToString();
                    }
                }
                catch
                {
                    // If not JSON or parsing fails, do nothing
                }
            }

            context.Response.Body = originalBodyStream;
            await context.Response.WriteAsync(modifiedBody, Encoding.UTF8);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}